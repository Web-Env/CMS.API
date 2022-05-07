using CMS.API.Infrastructure.Consts;
using CMS.API.Infrastructure.Email;
using CMS.API.Infrastructure.Email.ViewModels;
using CMS.API.Infrastructure.Encryption;
using CMS.API.Infrastructure.Encryption.Helpers;
using CMS.API.Infrastructure.Enums;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Infrastructure.Settings;
using CMS.API.UploadModels.User;
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
        public static async Task<bool> CheckUserExistsByIdAsync(Guid userId, IUserRepository userRepository)
        {
            var user = await GetUserByIdAsync(userId, userRepository).ConfigureAwait(false);
            return user != null;
        }

        public static async Task<bool> CheckUserIsAdminByIdAsync(Guid userId, IUserRepository userRepository)
        {
            var user = await GetUserByIdAsync(userId, userRepository).ConfigureAwait(false);
            return user.IsAdmin;
        }

        public static async Task<Domain.Entities.User> GetUserByIdAsync(Guid userId, IUserRepository userRepository)
        {
            var user = await userRepository.GetByIdAsync(userId);
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
            Guid requesterId,
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

            user.CreatedBy = requesterId;
            user.CreatedOn = DateTime.Now;
            user.LastUpdatedBy = requesterId;
            user.LastUpdatedOn = DateTime.Now;

            await repositoryManager.UserRepository.AddAsync(user);

            await CreateDataForNewUserAsync(
                user.Email,
                requesterAddress,
                repositoryManager,
                smtpSettings,
                emailSettings).ConfigureAwait(false);

            return user;
        }

        public static async Task<Domain.Entities.User> UpdateUserAsync(
            UserUploadModel updatedUser,
            Guid requesterId,
            IUserRepository userRepository)
        {
            var userToBeUpdated = await userRepository.GetByIdAsync(updatedUser.Id);

            if (userToBeUpdated != null)
            {
                if (updatedUser.IsAdmin && !userToBeUpdated.IsAdmin)
                {
                    if (updatedUser.AdminPassword != null)
                    {
                        var requesterIsValidAdmin = await CheckUserCredentialsValidAsync(
                            userRepository,
                            requesterId,
                            updatedUser.AdminPassword).ConfigureAwait(false);

                        if (!requesterIsValidAdmin)
                        {
                            throw new InvalidTokenException(
                                InvalidTokenType.TokenNotFound,
                                "The provided Administrator Password is invalid");
                        }
                    }
                    else
                    {
                        throw new InvalidTokenException(
                            InvalidTokenType.TokenNotFound,
                            "The provided Administrator Password is invalid");
                    }
                }

                userToBeUpdated.FirstName = updatedUser.FirstName;
                userToBeUpdated.LastName = updatedUser.LastName;
                userToBeUpdated.ExpiresOn = updatedUser.ExpiresOn;
                userToBeUpdated.IsAdmin = updatedUser.IsAdmin;
                userToBeUpdated.LastUpdatedOn = DateTime.Now;
                userToBeUpdated.LastUpdatedBy = requesterId;

                return await userRepository.UpdateAsync(userToBeUpdated);
            }
            else
            {
                throw new UserDoesNotExistException("User not found with provided Id", "User not found with provided Id");
            }
        }

        public static async Task DeleteUserAsync(
            Guid userId,
            Guid requesterId,
            IUserRepository userRepositoryManager)
        {
            var user = await GetUserByIdAsync(userId, userRepositoryManager).ConfigureAwait(false);

            if (user != null)
            {
                user.Deleted = true;
                user.LastUpdatedOn = DateTime.Now;
                user.LastUpdatedBy = requesterId;

                await userRepositoryManager.UpdateAsync(user);
            }
            else
            {
                throw new InvalidTokenException(InvalidTokenType.TokenNotFound, "User not found");
            }
        }

        public static async Task CreateDataForNewUserAsync(
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
                var emailService = new EmailService();
                var resetIdentifier = ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.IdentifierLength);
                var hashedResetIdentifier = HashingHelper.HashIdentifier(resetIdentifier);
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

                var accountSetupViewModel = new LinkEmailViewModel
                {
                    FullName = $"{user.FirstName} {user.LastName}",
                    UrlDomain = emailSettings.PrimaryRedirectDomain,
                    Link = encodedEncryptedIdentifier
                };

                await repositoryManager.PasswordResetRepository.AddAsync(passwordReset);

                var accountSetupMessage = emailService.CreateHtmlMessage(
                    smtpSettings,
                    $"{user.FirstName} {user.LastName}",
                    user.Email,
                    "Welcome",
                    EmailCreationHelper.CreateWelcomeVerificationEmailString(accountSetupViewModel));

                await emailService.SendEmailAsync(smtpSettings, accountSetupMessage);
            }
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
                var passwordResetViewModel = new LinkEmailViewModel
                {
                    FullName = $"{user.FirstName} {user.LastName}",
                    UrlDomain = emailSettings.PrimaryRedirectDomain,
                    Link = encodedEncryptedIdentifier
                };

                await repositoryManager.PasswordResetRepository.AddAsync(passwordReset);

                var passwordResetMessage = emailService.CreateHtmlMessage(
                    smtpSettings,
                    $"{user.FirstName} {user.LastName}",
                    user.Email,
                    "Reset Your Password",
                    EmailCreationHelper.CreatePasswordResetEmailString(passwordResetViewModel));

                await emailService.SendEmailAsync(smtpSettings, passwordResetMessage);
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
