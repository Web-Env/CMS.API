using System.IO;
using System.Text;

namespace CMS.API.Mailer.Helpers
{
    public static class EmailCreationHelper
    {
        public static string NewUserCreateEmail(string firstName, string lastName, string organisationName, string organisationUrl, string passwordSetLink)
        {
            var htmlString = File.ReadAllText("Templates/UserWelcomeEmail.html", Encoding.UTF8);

            return FormatHtmlString(htmlString, firstName, lastName, organisationName, organisationUrl, passwordSetLink);
        }

        private static string FormatHtmlString(string htmlString, params string[] arguments)
        {
            var argIndex = 0;
            foreach (var arg in arguments) 
            {
                htmlString = htmlString.Replace("{" + argIndex + "}", arg);

                argIndex++;
            }

            return htmlString;
        }
    }
}
