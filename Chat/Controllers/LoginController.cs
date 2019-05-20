using Chat.Helpers;
using Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Threading.Tasks;


namespace Chat.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LoginController : ControllerBase
  {

    private readonly UserManager<User> _userManager;
    private readonly IAuthenticationService _authService;

    private readonly string _badUser = "This user doesn't exist.";
    private object _badPassword = "You've entered the wrong password.";
    private object _badToken = "Your refresh token is invalid.";

    public LoginController(UserManager<User> userManager,
      IAuthenticationService authenticationService)
    {
      _userManager = userManager;
      _authService = authenticationService;
    }

    // POST: api/Login
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Post([FromBody] DTOCredentials credentials)
    {
      if (credentials == null) return BadRequest();

      // get user from db
      var user = await _userManager.FindByNameAsync(credentials.Username);
      if (user == null) return BadUserRequest();

      var result = await _authService.Authenticate(user, credentials);

      if (result == null) return BadPassword();
      return Ok(result);
    }

    [HttpPut("refreshToken")]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] DTOUser dtoUser)
    {
      if (dtoUser == null) return BadRequest();

      // get user from db
      var user = await _userManager.FindByNameAsync(dtoUser.UserName);
      if (user == null) return BadUserRequest();

      var result = await _authService.RefreshAccessToken(user, dtoUser.RefreshToken);

      if (result == null) return BadToken();
      return Ok(result);
    }

    private IActionResult BadToken() => Unauthorized(_badToken);

    private IActionResult BadUserRequest() => BadRequest(_badUser);

    private IActionResult BadPassword() => Unauthorized(_badPassword);  
  }
}
