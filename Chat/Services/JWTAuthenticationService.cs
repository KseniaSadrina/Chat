using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Chat.DAL;
using Chat.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.Helpers;

namespace Chat.Services
{
  public class JWTAuthenticationService : IAuthenticationService
  {
    #region Private Fields

    private readonly ChatContext _context;
    private readonly TokenConfiguration _config;
    private readonly UserManager<User> _userManager;
    private readonly string _jwt = "JWT";

    #endregion

    #region Initalization and CTOR

    public JWTAuthenticationService(ChatContext context,
      IOptions<TokenConfiguration> options,
      UserManager<User> userManager)
    {
      _context = context;
      _config = options.Value;
      _userManager = userManager;
    }

    #endregion

    # region IAuthenticationService Implementations

    public async Task<DTOUser> Authenticate(User user, DTOCredentials credentials)
    {
      if (string.IsNullOrEmpty(credentials.Password)) return null;

      // get user salt from db
      var salt = await _context.Salts.FirstOrDefaultAsync(s => s.UserId == user.Id);
      if (salt == null) return null;

      // convert salt to bytes
      var byteSalt = Convert.FromBase64String(salt.Secret);
      var hashed = credentials.Password.HashString(byteSalt);

      if (hashed != user.PasswordHash) return null;

      // authentication successful so generate jwt access, refresh tokens and the role of the user

      var accessToken = GenerateAccessToken(user);
      var refreshToken = GenerateRefreshToken(user);
      var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

      // save the refresh token so that the user can aquire a new access token

      if ((await _userManager.GetAuthenticationTokenAsync(user, _config.Issuer, _jwt)) != null)
        await _userManager.RemoveAuthenticationTokenAsync(user, _config.Issuer, _jwt);

      var x = await _userManager.SetAuthenticationTokenAsync(user, _config.Issuer, _jwt, refreshToken);
      

      return new DTOUser(user, accessToken, refreshToken, role);
    }

    public async Task<string> RefreshAccessToken(User user, string refreshToken)
    {
      var savedToken = await _userManager.GetAuthenticationTokenAsync(user, _config.Issuer, _jwt);
      if (savedToken != refreshToken) return null;

      return GenerateAccessToken(user);
    }

    #endregion

    #region Private Token Generation Methods

    private string GenerateToken(User user, double? exp= null)
    {
      var key = Encoding.ASCII.GetBytes(_config.Secret);
      var claim = new[] { new Claim(ClaimTypes.Name, user.UserName) };
      JwtSecurityToken jwtToken;
      if (exp == null)
        jwtToken = new JwtSecurityToken(
                _config.Issuer,
                _config.Audience,
                claim,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );
      else
        jwtToken = new JwtSecurityToken(
                _config.Issuer,
                _config.Audience,
                claim,
                expires: DateTime.Now.AddDays(exp ?? 0),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            );

      var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

      return token;
    }

    private string GenerateRefreshToken(User user) => GenerateToken(user);

    private string GenerateAccessToken(User user) => GenerateToken(user, _config.AccessExpiration);

    #endregion

  }

}
