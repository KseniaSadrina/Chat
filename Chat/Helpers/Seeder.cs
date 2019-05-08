using Chat.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Helpers
{
  public static class Seeder
  {
    public static async Task SeedUserRoles(this UserManager<User> userManager, ILogger logger)
    {
      var usersToRoles = new Dictionary<User, string>{
        {
          new User()
        {
          FirstName = Consts.ADMINNAME,
          LastName = Consts.ADMINNAME,
          UserName = Consts.ADMINNAME,
          UserType = Models.Enums.UserType.Human,
          Email = "sdfdsf@sdfdsf.com"
        }
        , Roles.Administrator.ToString() },
        {  new User()
        {
          FirstName = Consts.MARLEYNAME,
          LastName = Consts.MARLEYNAME,
          UserName = Consts.MARLEYNAME,
          UserType = Models.Enums.UserType.Bot,
          Email = "sdfdsf@sdfdsf.com"
        }, Roles.Bot.ToString() }
      };
      foreach (var user in usersToRoles.Keys)
      {
        // check if this user isn't already exist
        if (userManager.FindByNameAsync(user?.UserName).Result == null)
        {
          logger.LogDebug($"Seeding User {user?.UserName}.");
          await userManager.CreateAsync(user);
          var roles = await userManager.GetRolesAsync(user);
          var role = usersToRoles[user];
          try
          {
            if (roles == null || !(roles.Contains(role)))
              await userManager.AddToRoleAsync(user, role);
          }
          catch (Exception ex)
          {
            logger.LogError($"Failed seeding user {user?.UserName} with role {usersToRoles[user]}: ", ex);
          }
        }
      }
    }

    public static async Task SeedRoles(this RoleManager<Role> roleManager, ILogger logger)
    {
      foreach (var role in Enum.GetValues(typeof(Roles)).Cast<Roles>())
      {
        var roleName = role.ToString();
        logger.LogDebug($"Seeding Role {roleName}");
        if (roleManager.FindByNameAsync(roleName).Result == null)
          await roleManager.CreateAsync(new Role { Name = roleName });
      }
    }
  }
}
