using CMS.Domain.Enums;
using CMS.Domain.Repositories;
using CMS.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CMS.API.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        public IRepositoryManager RepositoryManager { get; private set; }

        public CustomControllerBase(IRepositoryManager repositoryManager)
        {
            RepositoryManager = repositoryManager;
        }

        private async Task LogAction(UserActionCategory actionCategory, UserAction action, Guid guid, string userAddress, DateTime occurredOn)
        {
        }
    }
}
