using CMS.API.Infrastructure.Consts;
using CMS.API.Infrastructure.Encryption;
using CMS.API.Infrastructure.Encryption.Helpers;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Infrastructure.Settings;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using CMS.Domain.Repositories.User.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebEnv.Util.Mailer;
using WebEnv.Util.Mailer.Settings;

namespace CMS.API.Models.User
{
    public static class UserModel
    {
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

        public static async Task CreateNewUserAsync(
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
                    "A User with this email address already exists",
                    "A User with this email address already exists"
                ));
            }

            user.Password = HashingHelper.HashPassword(user.Password);
            user.UserSecret = EncryptionService.EncryptString(ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.IdentifierLength));
            user.CreatedOn = DateTime.Now;

            try
            {
                await repositoryManager.UserRepository.AddAsync(user);

                await CreateNewVerficationAsync(
                    user.Email,
                    requesterAddress,
                    repositoryManager,
                    smtpSettings,
                    emailSettings,
                    isFirstContact: true).ConfigureAwait(false);

            }
            catch (Exception)
            {
                throw;
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

                try
                {
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
                        RequesterAddress = requesterAddress
                    };
                    //var verificationViewModel = new LinkEmailViewModel
                    //{
                    //    FullName = $"{user.FirstName} {user.LastName}",
                    //    UrlDomain = emailSettings.PrimaryRedirectDomain,
                    //    Link = verificationIdentifier
                    //};

                    //await repositoryManager.UserVerificationRepository.AddAsync(verification);

                    //var verificationMessage = emailService.CreateHtmlMessage(
                    //    smtpSettings,
                    //    $"{user.FirstName} {user.LastName}",
                    //    user.Email,
                    //    isFirstContact ? "Welcome to Shufl" : "Verify Your Account",
                    //    isFirstContact ?
                    //        EmailCreationHelper.CreateWelcomeVerificationEmailString(verificationViewModel) :
                    //        EmailCreationHelper.CreateVerificationEmailString(verificationViewModel));

                    //await emailService.SendEmailAsync(smtpSettings, verificationMessage);

                }
                catch (Exception)
                {
                    throw;
                }
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
    }
}
