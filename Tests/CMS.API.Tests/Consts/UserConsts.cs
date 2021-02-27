namespace CMS.API.Tests.Consts
{
    public static class UserConsts
    {
        public static string RootUserId { get; } = "7d64b439-9840-4acb-98c6-07ad9e2d18ce";
        public static string RootUserIdEncrypted { get; } = "vnRX7sghKl47BfL9B7kmbCOFzHvB9qgUd+YmNqg35cJ6ainXSH4kKf495B2e1VaZxD4wXmezPvS2H/FakxiMrhmo/ormFpAzTmO7ExICeFxa7ZGZWb57oq1n4A2QhXCZIf7f1Pgh9XSNWV3KLxdB/6tnfmCuZfvxEJRiH5kqX3uPRrcH+ZolnIXZ+ZGIz5ZEsa9f0JTokYwgDp+rx9N9IvuHM+ecs+OESYujpKtsnprLRI8EOBwipBDEIiA3D8DKgkPPRkEWuOgozr/W86sDkd44Bvlr3fxWGY/qM8lH/8acp4YSWX38ZE97QF5cM5GqMyq4rHxGVF5zdkWdVj7C8A==";
        public static string RootUserEmail { get; } = "root@cms.com";
        public static string RootUserPlainTextPassword { get; } = "RootUserPassword";
        public static string RootUserHashedPassword { get; } = "7EA5F20E6EED946CAAD20766ACC70B8705FACD6ED2DEFC203A63AD3612A76CE2"; //RootUserPassword
        public static string RootUserFirstName { get; } = "Root";
        public static string RootUserLastName { get; } = "Rooterson";

        public static string TestUserPassword { get; } = "B6087C3884A4323CB1F1BB663E33EA355FD99B224FC101E76322E98B1660883E"; //TestUserPassword
        public static string TestUserFirstName { get; } = "Test";
        public static string TestUserLastName { get; } = "Testerson";

        public static string DefaultAddress { get; } = "255.255.255.255";
    }
}
