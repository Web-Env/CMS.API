using CMS.API.Tests.Consts;
using CMS.Domain.Entities;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CMS.API.Tests.Funcs
{
    public static class UserFunc
    {
        public static async Task CreateRootUser(CMSContext context)
        {
            if (context.Users.Find(Guid.Parse(UserConsts.RootUserId)) == null)
            {
                SHA256Managed sha256Hasher = new SHA256Managed();
                UTF8Encoding encoder = new UTF8Encoding();
                var hashedPassword = sha256Hasher.ComputeHash(encoder.GetBytes(UserConsts.RootUserHashedPassword));

                var user = new User
                {
                    Id = Guid.Parse(UserConsts.RootUserId),
                    Email = UserConsts.RootUserEmail,
                    Password = hashedPassword,
                    FirstName = UserConsts.RootUserFirstName,
                    LastName = UserConsts.RootUserLastName,
                    IsAdmin = true,
                    CreatedOn = DateTime.Now,
                    CreatedBy = Guid.Empty,
                    LastUpdatedOn = DateTime.Now,
                    LastUpdatedBy = Guid.Empty
                };

                context.Users.Add(user);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
