using Chat.DAL;
using Chat.Services;
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

    public static async Task SeedScenarios(this ChatContext context, IGuideService guideService, ILogger logger)
    {
      var all = guideService.GetAllScenarioGuide();
      foreach (var scenarioGuide in all)
      {
        logger.LogDebug($"Seeding scenario scenario {scenarioGuide.ScenarioId}");
        try
        {
          var scenario = new Scenario()
          {
            Id = scenarioGuide.ScenarioId,
            Name = scenarioGuide.ScenarioName,
            Description = scenarioGuide.Abstract
          };
          if (context.Scenarios.FirstOrDefault(item => item.Id == scenario.Id) != null )
            continue;
          context.Add(scenario);
          var i = 1;
          foreach (var goalGuide in scenarioGuide.Goals)
          {
            logger.LogDebug($"Seeding scenario scenario {goalGuide.Id}");
            var goal = new Goal()
            {
              Id = goalGuide.Id,
              Order = i,
              ScenarioId = scenarioGuide.ScenarioId,
              Text = goalGuide.GoalName
            };
            if (context.Goals.FirstOrDefault(item => item.Id == goal.Id) != null)
              continue;
            context.Add(goal);
            i++;
          }
        }
        catch (Exception ex)
        {
          logger.LogError("Failed duering addition of new scenario.", ex);
        }
      }
      await context.SaveChangesAsync();
    }
  }
}
