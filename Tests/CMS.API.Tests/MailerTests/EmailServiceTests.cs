using CMS.API.Infrastructure.Settings;
using CMS.API.Mailer;
using CMS.API.Mailer.Helpers;
using CMS.API.Tests.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace CMS.API.Tests.MailerTests
{
    [Trait("Category", "Unit")]
    public class EmailServiceTests
    {
        private readonly EmailService _emailService;
        private readonly OrganisationSettings _organisationSettings;

        public EmailServiceTests()
        {
            _emailService = new EmailService(EmailServiceHelper.GetSmtpSettings());
            _organisationSettings = EmailServiceHelper.GetOrganisationSettings();
        }

        [Fact]
        public async Task SendEmail_ShouldNotReturnAnError()
        {
            //Arrange
            var emailBody = EmailCreationHelper.NewUserCreateEmail(
                "Test",
                "Testerson", 
                _organisationSettings.OrganisationName,
                _organisationSettings.OrganisationUrl, 
                "LJKNASDF309NMFO123IH130IHFN31S34"
            );

            //Act
            var smtpResult = await _emailService.SendEmail("Test Testerson", "noreply@webenv.io", $"Welcome to {_organisationSettings.OrganisationName}", emailBody);

            //Assert
            Assert.True(smtpResult);
        }

        [Fact]
        public async Task SendEmail_ShouldReturnAnErrorWhenSmtpSettingsInvalid()
        {
            //Arrange
            var smtpSettings = EmailServiceHelper.GetSmtpSettings();
            smtpSettings.EmailSmtpHost = "";
            var emailService = new EmailService(smtpSettings);
            var emailBody = EmailCreationHelper.NewUserCreateEmail(
                "Test",
                "Testerson",
                _organisationSettings.OrganisationName,
                _organisationSettings.OrganisationUrl,
                "LJKNASDF309NMFO123IH130IHFN31S34"
            );

            //Act
            var smtpResult = await emailService.SendEmail("Test Testerson", "noreply@webenv.io", $"Welcome to {_organisationSettings.OrganisationName}", emailBody);

            //Assert
            Assert.False(smtpResult);
        }
    }
}
