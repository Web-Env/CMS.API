﻿using CMS.API.DownloadModels.Auth;
using CMS.API.Infrastructure.Encryption;
using CMS.API.Infrastructure.Encryption.Certificates;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.UploadModels.Auth;
using CMS.Domain.Repositories.User.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.API.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest model, IUserRepository userRepository)
        {
            var user = (await userRepository.FindAsync(u =>
                            u.Email == model.Email &&
                            !u.Deleted
                        )).FirstOrDefault();

            if (user != null)
            {
                if (user.ExpiresOn != null && user.ExpiresOn < DateTime.Now)
                {
                    throw new AuthenticationException
                    (
                        "Invalid login",
                        "No user found with the provided login details"
                    );
                }

                var passwordIsCorrect = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
                if (passwordIsCorrect)
                {
                    var encryptedUserId = EncryptionService.EncryptUserId(user.Id);
                    var authenticationResponse = new AuthenticationResponse
                    {
                        UserId = encryptedUserId,
                        Token = GenerateJwtToken(user.Id.ToString(), user.UserSecret),
                        Super = user.IsAdmin,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    };

                    return authenticationResponse;
                }
                else
                {
                    throw new AuthenticationException
                    (
                        "Invalid login",
                        "No user found with the provided login details"
                    );
                }
            }
            else
            {
                throw new AuthenticationException
                (
                    "Invalid login",
                    "No user found with the provided login details"
                );
            }
        }

        private string GenerateJwtToken(string userId, string userSecret)
        {
            SecurityTokenDescriptor tokenDescriptor = GetTokenDescriptor(userId, userSecret);

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        private SecurityTokenDescriptor GetTokenDescriptor(string userId, string userSecret)
        {
            const int expiringDays = 365;
            var signingAudienceCertificate = new SigningAudienceCertificate();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userId),
                    new Claim(ClaimTypes.NameIdentifier, userSecret)
                }),
                Expires = DateTime.UtcNow.AddDays(expiringDays),
                SigningCredentials = signingAudienceCertificate.GetAudienceSigningKey()
            };

            return tokenDescriptor;
        }
    }
}
