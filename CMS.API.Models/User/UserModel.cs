using CMS.API.Infrastructure.Consts;
using CMS.API.Infrastructure.Email;
using CMS.API.Infrastructure.Email.ViewModels;
using CMS.API.Infrastructure.Encryption;
using CMS.API.Infrastructure.Encryption.Helpers;
using CMS.API.Infrastructure.Enums;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Infrastructure.Settings;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using CMS.Domain.Repositories.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEnv.Util.Mailer;
using WebEnv.Util.Mailer.Settings;

namespace CMS.API.Models.User
{
    public static class UserModel
    {
        public static async Task<Domain.Entities.User> GetUserById(Guid userId, IUserRepository userRepository)
        {
            return await userRepository.GetByIdAsync(userId);
        }

        public static async Task<bool> CheckUserExistsByIdAsync(Guid userId, IUserRepository userRepository)
        {
            var user = await GetUserById(userId, userRepository).ConfigureAwait(false);
            return user != null;
        }

        public static async Task<bool> CheckUserIsAdminByIdAsync(Guid userId, IUserRepository userRepository)
        {
            var user = await GetUserById(userId, userRepository).ConfigureAwait(false);
            return user.IsAdmin;
        }

        public static async Task<Domain.Entities.User> GetUserByIdAsync(Guid userId, IUserRepository userRepository)
        {
            var user = await GetUserById(userId, userRepository).ConfigureAwait(false);
            return user;
        }

        private static async Task<(bool exists, Domain.Entities.User user)> CheckUserExistsWithEmailAsync(
            string email,
            IUserRepository userRepository)
        {
            var existingEmail = await userRepository.FindAsync(u => u.Email == email);
            if (existingEmail.Any())
            {
                return (true, existingEmail.FirstOrDefault());
            }

            return (false, null);
        }

        public static async Task<bool> CheckUserCredentialsValidAsync(IUserRepository userRepository, Guid userId, string password)
        {
            var user = (await userRepository.FindAsync(u =>
                            u.Id == userId
                        )).FirstOrDefault();

            if (user != null)
            {
                var passwordIsCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);

                return passwordIsCorrect;
            }

            return false;
        }

        public static async Task<IEnumerable<VGetUser>> GetUsersPageAsync(IUserRepository userRepository, int page, int pageSize)
        {
            var users = await userRepository.GetPageAsync(page, pageSize);
            return users;
        }

        public static async Task<Domain.Entities.User> CreateNewUserAsync(
            Domain.Entities.User user,
            string requesterAddress,
            IRepositoryManager repositoryManager,
            SmtpSettings smtpSettings,
            EmailSettings emailSettings)
        {
            var (emailExists, _) = await CheckUserExistsWithEmailAsync(user.Email, repositoryManager.UserRepository)
                .ConfigureAwait(false);

            if (emailExists)
            {
                throw (new EmailAlreadyRegisteredException(
                    "A user with this email address already exists",
                    "A user with this email address already exists"
                ));
            }

            user.UserSecret = EncryptionService.EncryptString(ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.IdentifierLength));
            user.CreatedOn = DateTime.Now;

            await repositoryManager.UserRepository.AddAsync(user);

            await CreateNewVerficationForNewUserAsync(
                user.Email,
                requesterAddress,
                repositoryManager,
                smtpSettings,
                emailSettings).ConfigureAwait(false);

            return user;
        }

        public static async Task CreateNewVerficationForNewUserAsync(
           string email,
           string requesterAddress,
           IRepositoryManager repositoryManager,
           SmtpSettings smtpSettings,
           EmailSettings emailSettings
           )
        {
            var (exists, user) = await CheckUserExistsWithEmailAsync(email, repositoryManager.UserRepository)
                .ConfigureAwait(false);

            if (exists)
            {
                if (user.IsVerified)
                {
                    throw new UserAlreadyVerifiedException("User has already been verified", "User has already been verified");
                }

                var emailService = new EmailService();
                var resetIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.IdentifierLength);
                var verificationIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.IdentifierLength);
                var hashedResetIdentifier = HashingHelper.HashIdentifier(resetIdentifier);
                var hashedVerificationIdentifier = HashingHelper.HashIdentifier(verificationIdentifier);
                var encryptedUserId = EncryptionService.EncryptString(user.Id.ToString());
                var encryptedIdentifier = EncryptionService.EncryptString(resetIdentifier);
                var encodedEncryptedIdentifier = System.Web.HttpUtility.UrlEncode(encryptedIdentifier);

                var passwordReset = new PasswordReset
                {
                    Identifier = hashedResetIdentifier,
                    UserId = encryptedUserId,
                    ExpiryDate = DateTime.Now.AddDays(7),
                    RequesterAddress = requesterAddress,
                    Active = true,
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now
                };

                var verification = new UserVerification
                {
                    Identifier = hashedVerificationIdentifier,
                    UserId = user.Id,
                    ExpiryDate = DateTime.Now.AddDays(7),
                    RequesterAddress = requesterAddress,
                    Active = true,
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now
                };

                var verificationViewModel = new LinkEmailViewModel
                {
                    FullName = $"{user.FirstName} {user.LastName}",
                    UrlDomain = emailSettings.PrimaryRedirectDomain,
                    Link = encodedEncryptedIdentifier
                };

                await repositoryManager.PasswordResetRepository.AddAsync(passwordReset);
                await repositoryManager.UserVerificationRepository.AddAsync(verification);

                var verificationMessage = emailService.CreateHtmlMessage(
                    smtpSettings,
                    $"{user.FirstName} {user.LastName}",
                    user.Email,
                    "Welcome",
                    EmailCreationHelper.CreateWelcomeVerificationEmailString(verificationViewModel));

                await emailService.SendEmailAsync(smtpSettings, verificationMessage);
            }
        }

        public static async Task CreateNewVerficationAsync(
            string email,
            string requesterAddress,
            IRepositoryManager repositoryManager,
            SmtpSettings smtpSettings,
            EmailSettings emailSettings,
            bool isFirstContact = false
            )
        {
            var (exists, user) = await CheckUserExistsWithEmailAsync(email, repositoryManager.UserRepository)
                .ConfigureAwait(false);

            if (exists)
            {
                if (user.IsVerified)
                {
                    throw new UserAlreadyVerifiedException("User has already been verified", "User has already been verified");
                }

                if (!isFirstContact)
                {
                    await DeactivateExistingUserVerificationsAsync(user.Id, repositoryManager.UserVerificationRepository)
                        .ConfigureAwait(false);
                }

                var emailService = new EmailService();
                var verificationIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.IdentifierLength);
                var hashedVerificationIdentifier = HashingHelper.HashIdentifier(verificationIdentifier);

                var verification = new UserVerification
                {
                    Identifier = hashedVerificationIdentifier,
                    UserId = user.Id,
                    ExpiryDate = DateTime.Now.AddDays(7),
                    RequesterAddress = requesterAddress,
                    Active = true,
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now
                };
                var verificationViewModel = new LinkEmailViewModel
                {
                    FullName = $"{user.FirstName} {user.LastName}",
                    UrlDomain = emailSettings.PrimaryRedirectDomain,
                    Link = verificationIdentifier
                };

                await repositoryManager.UserVerificationRepository.AddAsync(verification);

                var verificationMessage = emailService.CreateHtmlMessage(
                    smtpSettings,
                    $"{user.FirstName} {user.LastName}",
                    user.Email,
                    isFirstContact ? "Welcome" : "Verify Your Account",
                    isFirstContact ?
                        EmailCreationHelper.CreateWelcomeVerificationEmailString(verificationViewModel) :
                        EmailCreationHelper.CreateVerificationEmailString(verificationViewModel));

                await emailService.SendEmailAsync(smtpSettings, verificationMessage);
            }
        }

        private static async Task DeactivateExistingUserVerificationsAsync(Guid userId, IUserVerificationRepository verificationRepository)
        {
            var userVerifications = await verificationRepository.FindAsync(uv => uv.UserId == userId && (bool)uv.Active);

            if (userVerifications.Any())
            {
                foreach (UserVerification userVerification in userVerifications)
                {
                    userVerification.Active = false;
                    userVerification.LastUpdatedOn = DateTime.Now;
                }

                await verificationRepository.UpdateRangeAsync(userVerifications);
            }
        }

        public static async Task VerifyUserAsync(
            string verificationIdentifier,
            string requesterAddress,
            IRepositoryManager repositoryManager)
        {
            verificationIdentifier = HashingHelper.HashIdentifier(verificationIdentifier);
            var verificationIdentifierIsValid = await ValidateVerificationIdentifierAsync(
                    verificationIdentifier,
                    repositoryManager.UserVerificationRepository).ConfigureAwait(false);

            if (verificationIdentifierIsValid)
            {
                var userVerification = await repositoryManager.UserVerificationRepository.FindByIdentifierAsync(verificationIdentifier);
                var user = await repositoryManager.UserRepository.GetByIdAsync(userVerification.UserId);

                userVerification.Active = false;
                userVerification.UsedOn = DateTime.Now;
                userVerification.UsedByAddress = requesterAddress;
                user.IsVerified = true;

                await repositoryManager.UserVerificationRepository.UpdateAsync(userVerification);
                await repositoryManager.UserRepository.UpdateAsync(user);
            }
            else
            {
                throw new InvalidTokenException(InvalidTokenType.TokenNotFound, "The User Verification Identifier is invalid");
            }
        }

        public static async Task<bool> ValidateVerificationIdentifierAsync(
            string verificationIdentifier,
            IUserVerificationRepository userVerificationRepository)
        {
            var userVerification = await userVerificationRepository.FindByIdentifierAsync(
                verificationIdentifier);

            if (userVerification != null)
            {
                return IsUserVerificationValid(userVerification);
            }

            return false;
        }

        private static bool IsUserVerificationValid(UserVerification userVerification)
        {
            if (userVerification.ExpiryDate < DateTime.Now)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenExpired, "Token has expired");
            }

            if (userVerification.UsedOn != null || userVerification.UsedByAddress != null)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenUsed, "Token has been used");
            }

            if (userVerification.Active != null && !(bool)userVerification.Active)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenInactive, "Token is no longer active");
            }

            return true;
        }

        public static async Task<bool> ValidatePasswordResetTokenAsync(
            string passwordResetToken,
            IPasswordResetRepository passwordResetRepository)
        {
            var decryptedResetToken = DecryptionService.DecryptString(passwordResetToken);
            var hashedResetIdentifier = HashingHelper.HashIdentifier(decryptedResetToken);
            var (existsAndValid, _) = await CheckPasswordResetIdentifierExistsAndIsValidAsync(
                hashedResetIdentifier,
                passwordResetRepository).ConfigureAwait(false);

            return existsAndValid;
        }

        public static async Task ResetPasswordAsync(
            string passwordResetToken,
            string newPassword,
            string requesterAddress,
            IRepositoryManager repositoryManager)
        {
            var decryptedResetToken = DecryptionService.DecryptString(passwordResetToken);
            var hashedResetIdentifier = HashingHelper.HashIdentifier(decryptedResetToken);
            var (existsAndValid, passwordReset) = await CheckPasswordResetIdentifierExistsAndIsValidAsync(
                hashedResetIdentifier,
                repositoryManager.PasswordResetRepository).ConfigureAwait(false);

            if (existsAndValid)
            {
                var userId = Guid.Parse(DecryptionService.DecryptString(passwordReset.UserId));
                var user = await repositoryManager.UserRepository.GetByIdAsync(userId);

                user.Password = HashingHelper.HashPassword(newPassword);
                user.UserSecret = EncryptionService.EncryptString(ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.IdentifierLength));
                user.LastUpdatedOn = DateTime.Now;
                user.LastUpdatedBy = userId;

                passwordReset.Active = false;
                passwordReset.UsedOn = DateTime.Now;
                passwordReset.UsedByAddress = requesterAddress;
                passwordReset.LastUpdatedOn = DateTime.Now;

                await repositoryManager.UserRepository.UpdateAsync(user);
                await repositoryManager.PasswordResetRepository.UpdateAsync(passwordReset);
            }
            else
            {
                throw new InvalidTokenException(InvalidTokenType.TokenNotFound, "The Password Reset Token is invalid");
            }
        }

        private static async Task<(bool existsAndValid, PasswordReset passwordReset)> CheckPasswordResetIdentifierExistsAndIsValidAsync(
            string resetIdentifier,
            IPasswordResetRepository passwordResetRepository)
        {
            var passwordReset = await passwordResetRepository.FindByIdentifierAsync(resetIdentifier);

            if (passwordReset != null)
            {
                if (IsPasswordResetValid(passwordReset))
                {
                    return (true, passwordReset);
                }
            }

            return (false, null);
        }

        private static bool IsPasswordResetValid(PasswordReset passwordReset)
        {
            if (passwordReset.ExpiryDate < DateTime.Now)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenExpired, "Token has expired");
            }

            if (passwordReset.UsedOn != null || passwordReset.UsedByAddress != null)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenUsed, "Token has been used");
            }

            if (passwordReset.Active != null && !(bool)passwordReset.Active)
            {
                throw new InvalidTokenException(InvalidTokenType.TokenInactive, "Token is no longer active");
            }

            return true;
        }

        public static async Task CreateNewResetPasswordAsync(
            string email,
            string requesterAddress,
            IRepositoryManager repositoryManager,
            SmtpSettings smtpSettings,
            EmailSettings emailSettings)
        {
            var (exists, user) = await CheckUserExistsWithEmailAsync(email, repositoryManager.UserRepository)
                .ConfigureAwait(false);

            if (exists)
            {
                var emailService = new EmailService();
                var resetIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.IdentifierLength);
                var hashedResetIdentifier = HashingHelper.HashIdentifier(resetIdentifier);
                var encryptedUserId = EncryptionService.EncryptString(user.Id.ToString());
                var encryptedIdentifier = EncryptionService.EncryptString(resetIdentifier);
                var encodedEncryptedIdentifier = System.Web.HttpUtility.UrlEncode(encryptedIdentifier);

                await DeactivateExistingPasswordResetsAsync(encryptedUserId, repositoryManager.PasswordResetRepository)
                        .ConfigureAwait(false);

                var passwordReset = new PasswordReset
                {
                    Identifier = hashedResetIdentifier,
                    UserId = encryptedUserId,
                    ExpiryDate = DateTime.Now.AddHours(1),
                    RequesterAddress = requesterAddress,
                    Active = true,
                    CreatedOn = DateTime.Now,
                    LastUpdatedOn = DateTime.Now
                };
                var verificationViewModel = new LinkEmailViewModel
                {
                    FullName = $"{user.FirstName} {user.LastName}",
                    UrlDomain = emailSettings.PrimaryRedirectDomain,
                    Link = encodedEncryptedIdentifier
                };

                await repositoryManager.PasswordResetRepository.AddAsync(passwordReset);

                var verificationMessage = emailService.CreateHtmlMessage(
                    smtpSettings,
                    $"{user.FirstName} {user.LastName}",
                    user.Email,
                    "Reset Your Password",
                    EmailCreationHelper.CreatePasswordResetEmailString(verificationViewModel));

                await emailService.SendEmailAsync(smtpSettings, verificationMessage);
            }
        }

        private static async Task DeactivateExistingPasswordResetsAsync(string encryptedUserId, IPasswordResetRepository passwordResetRepository)
        {
            var userActivePasswordResets = await passwordResetRepository.FindAsync(x => x.UserId == encryptedUserId && (bool)x.Active);

            if (userActivePasswordResets.Any())
            {
                foreach (PasswordReset activePasswordReset in userActivePasswordResets)
                {
                    activePasswordReset.Active = false;
                    activePasswordReset.LastUpdatedOn = DateTime.Now;
                }

                await passwordResetRepository.UpdateRangeAsync(userActivePasswordResets);
            }
        }
    }
}
