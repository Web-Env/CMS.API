using CMS.API.DownloadModels;
using CMS.API.UploadModels;
using CMS.Domain.Repositories.Interfaces;
using System.Threading.Tasks;

namespace CMS.API.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest model, IUserRepository userRepository);
    }
}
