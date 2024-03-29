﻿using CMS.API.Infrastructure.Consts;
using CMS.API.Infrastructure.Encryption;
using CMS.API.Models;
using CMS.API.Tests.Consts;
using CMS.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace CMS.API.Tests.Funcs
{
    public static class UserFunc
    {
        public static async Task CreateRootUser(CMSContext context)
        {
            if (context.Users.Find(Guid.Parse(UserConsts.RootUserId)) == null)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(UserConsts.RootUserHashedPassword, workFactor: 12);

                var user = new User
                {
                    Id = Guid.Parse(UserConsts.RootUserId),
                    Email = UserConsts.RootUserEmail,
                    Password = hashedPassword,
                    UserSecret = EncryptionService.EncryptString(ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.IdentifierLength)),
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
