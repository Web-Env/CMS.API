namespace CMS.API.Mailer.Helpers
{
    public static class EmailCreationHelper
    {
        public static string NewUserCreateEmail(string name, string emailAdress, string passwordSetLink)
        {
            var htmlString = $"<h1>Welcome, {name}</h1><br><p>https://webenv.io/cms/password/set/{passwordSetLink}</p>";

            return htmlString;
        }
    }
}
