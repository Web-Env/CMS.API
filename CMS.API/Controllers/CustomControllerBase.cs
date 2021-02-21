using AutoMapper;
using CMS.API.UploadModels;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
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

        protected async Task LogAction(UserActionCategory actionCategory, UserAction action, Guid userId, string userAddress, DateTime occurredOn)
        {
            var auditLog = new AuditLog
            {
                ActionCategory = (short)actionCategory,
                Action = (short)action,
                UserId = userId,
                UserAddress = userAddress,
                OccurredOn = occurredOn
            };

            await RepositoryManager.AuditLogRepository.AddAsync(auditLog);
        }

        protected TDownloadModel MapEntityToDownloadModel<TEntity, TDownloadModel>(TEntity entity)
        {
            return _mapper.Map<TDownloadModel>(entity);
        }

        protected TEntity MapUploadModelToEntity<TEntity>(IUploadModel uploadModel)
        {
            return _mapper.Map<TEntity>(uploadModel);
        }
    }
}
