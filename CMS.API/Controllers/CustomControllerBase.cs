using AutoMapper;
using CMS.API.UploadModels;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.API.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        public IRepositoryManager RepositoryManager { get; private set; }
        private readonly IMapper _mapper;

        public CustomControllerBase(IRepositoryManager repositoryManager,
                                    IMapper mapper)
        {
            RepositoryManager = repositoryManager;
            _mapper = mapper;
        }

        protected async Task<AuditLog> LogAction(UserActionCategory actionCategory, UserAction action, Guid userId, string userAddress, DateTime occurredOn)
        {
            var auditLog = new AuditLog
            {
                ActionCategory = (short)actionCategory,
                Action = (short)action,
                UserId = userId,
                UserAddress = userAddress,
                OccurredOn = occurredOn
            };

            return await RepositoryManager.AuditLogRepository.AddAsync(auditLog);
        }

        protected TDownloadModel MapEntityToDownloadModel<TEntity, TDownloadModel>(TEntity entity)
        {
            return _mapper.Map<TDownloadModel>(entity);
        }

        protected TEntity MapUploadModelToEntity<TEntity>(IUploadModel uploadModel)
        {
            return _mapper.Map<TEntity>(uploadModel);
        }

        protected async Task<PasswordReset> GeneratePasswordSetLink(Guid userId, string requesterAddress)
        {
            var passwordReset = new PasswordReset
            {
                ResetIdentifier = GenerateResetIdentifier(),
                UserId = userId,
                Expiry = DateTime.Now.AddDays(15),
                RequesterAddress = requesterAddress,
                Active = true
            };

            await RepositoryManager.PasswordResetRepository.AddAsync(passwordReset);

            return passwordReset;
        }

        private string GenerateResetIdentifier()
        {
            var random = new Random();

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var resetIdentifier = new string(
                Enumerable.Repeat(chars, 32)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return resetIdentifier;
        }
    }
}
