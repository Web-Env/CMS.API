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

        public EmailServiceTests()
        {
            _emailService = new EmailService(EmailServiceHelper.GetSmtpSettings());
        }

        [Fact]
        public async Task SendEmail_ShouldNotReturnAnError()
        {
            //Arrange
            var emailBody = EmailCreationHelper.NewUserCreateEmail("Test Testerson", "adambarryodonovan@gmail.com", "LJKNASDF309NMFO123IH130IHFN31S34");

            //Act
            await _emailService.SendEmail("Test Testerson", "noreply@webenv.io", "Welcome", emailBody);

            //Assert
            Assert.True(true);
        }
    }
}
