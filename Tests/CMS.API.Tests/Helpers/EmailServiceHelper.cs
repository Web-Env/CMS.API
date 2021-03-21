using CMS.API.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;

namespace CMS.API.Tests.Helpers
{
    public static class EmailServiceHelper
    {
        public static SmtpSettings GetSmtpSettings()
        {
            var smtpConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            return new SmtpSettings
            {
                EmailFromName = smtpConfig["SmtpSettings:emailFromName"],
                EmailFromAddress = smtpConfig["SmtpSettings:emailFromAddress"],
                EmailSmtpHost = smtpConfig["SmtpSettings:emailSmtpHost"],
                EmailSmtpPort = int.Parse(smtpConfig["SmtpSettings:emailSmtpPort"]),
                EmailSmtpUsername = smtpConfig["SmtpSettings:emailSmtpUsername"],
                EmailSmtpPassword = smtpConfig["SmtpSettings:emailSmtpPassword"],
            };
        }

        public static OrganisationSettings GetOrganisationSettings()
        {
            var organisationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            return new OrganisationSettings
            {
                OrganisationName = organisationConfig["OrganisationSettings:organisationName"],
                OrganisationUrl = organisationConfig["OrganisationSettings:organisationUrl"]
            };
        }
    }
}
