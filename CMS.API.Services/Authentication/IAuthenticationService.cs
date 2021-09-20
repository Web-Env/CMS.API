using CMS.API.DownloadModels.Auth;
using CMS.API.UploadModels.Auth;
using CMS.Domain.Repositories.User.Interfaces;
using System.Threading.Tasks;

namespace CMS.API.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest model, IUserRepository userRepository);
    }
}
