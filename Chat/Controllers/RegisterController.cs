using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.Helpers;

namespace Chat.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [AllowAnonymous]
  public class RegisterController : ControllerBase
  {
    private readonly UserManager<User> _userManager;
    private readonly ILogger<RegisterController> _logger;
    private readonly ChatContext _context;

    public RegisterController(UserManager<User> userManager,
      ILogger<RegisterController> logger,
      ChatContext context)
    {
      _userManager = userManager;
      _logger = logger;
      _context = context;
    }

    // GET: api/Register/5
    [HttpGet]
    public async Task<bool> CanRegisterUsername(string userName)
    {
      if (string.IsNullOrEmpty(userName)) return false;
      var user = await _userManager.FindByNameAsync(userName);
      return user == null;
    }

    // POST: api/Register
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] RegistrationUser dtoUser)
    {
      if (dtoUser == null) return BadRequest();
      try
      {
        _logger.LogInformation($"Registering new user {dtoUser?.UserName}.");

        var salt = Hasher.GenerateSalt();
        var newUser = new User(dtoUser, salt);
        var result = await _userManager.CreateAsync(newUser); // save user
        if (result.Succeeded)
        {
          // save salt
          await _context.Salts.AddAsync(new Salt { UserId = newUser.Id, Secret = Convert.ToBase64String(salt) });
          await _context.SaveChangesAsync();

          result = await _userManager.AddToRoleAsync(newUser, dtoUser.Role); // add user to role
          if (result.Succeeded)
            return CreatedAtAction("Register", new { id = newUser.Id }, newUser);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError("Something went wrong during the addition of the user: ", ex);
      }

      return StatusCode(500);
    }
  }
}
