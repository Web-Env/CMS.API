namespace CMS.API.Tests.Consts
{
    public static class UserConsts
    {
        public static string RootUserId { get; } = "7d64b439-9840-4acb-98c6-07ad9e2d18ce";
        public static string RootUserEmail { get; } = "root@cms.com";
        public static string RootUserHashedPassword { get; } = "7EA5F20E6EED946CAAD20766ACC70B8705FACD6ED2DEFC203A63AD3612A76CE2"; //RootUserPassword
        public static string RootUserFirstName { get; } = "Root";
        public static string RootUserLastName { get; } = "Rooterson";

        public static string TestUserPassword { get; } = "B6087C3884A4323CB1F1BB663E33EA355FD99B224FC101E76322E98B1660883E"; //TestUserPassword
        public static string TestUserFirstName { get; } = "Test";
        public static string TestUserLastName { get; } = "Testerson";

        public static string DefaultAddress { get; } = "255.255.255.255";
    }
}
