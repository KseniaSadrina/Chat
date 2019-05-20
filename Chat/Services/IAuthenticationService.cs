using Models;
using System.Threading.Tasks;

namespace Chat.Services
{
  public interface IAuthenticationService
  {

    Task<DTOUser> Authenticate(User user, DTOCredentials credentials);

    Task<string> RefreshAccessToken(User user, string refreshToken);
  }
}
