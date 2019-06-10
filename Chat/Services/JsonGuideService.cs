using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;

namespace Chat.Services
{
  public class JsonGuideService : IGuideService
  {
    private readonly ILogger<JsonGuideService> _logger;
    private string _guidesLocation = "./Guides";

    public JsonGuideService(ILogger<JsonGuideService> logger)
    {
      _logger = logger;
    }

    public ScenarioGuide GetScenarioGuide(int scenarioId)
    {
      try
      {
        _logger.LogInformation($"Serializing scenario guide for scenario {scenarioId}");
        var strId = scenarioId.ToString();
        string guidePath = Path.Combine(_guidesLocation, $"{strId}.json");
        using (var ms = new StreamReader(guidePath))
        {
          // Deserialization from JSON  
          var guide = ms.ReadToEnd();
          return JsonConvert.DeserializeObject<ScenarioGuide>(guide);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError($"Failed with the following exception: ", ex);
        return null;
      }
    }

    public GoalGuide GetGoalGuide(Goal goal, ScenarioGuide scenarioGuide = null)
    {
      _logger.LogInformation($"Getting goal guide for scenario {goal?.Id}");
      if (goal == null) return null;
      if (scenarioGuide == null)
        scenarioGuide = GetScenarioGuide(goal.ScenarioId);
      return scenarioGuide.Goals.FirstOrDefault(g => g.Id == goal.Id);
    }

    public IEnumerable<ScenarioGuide> GetAllScenarioGuide()
    {
      var files = new DirectoryInfo(_guidesLocation).GetFiles("*.json");
      foreach (var file in files)
      {
        var id = int.Parse(file.Name.Split('.')[0]);
        yield return GetScenarioGuide(id);
      }

    }
  }
}
