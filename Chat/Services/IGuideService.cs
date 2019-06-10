using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Services
{
  public interface IGuideService
  {
    GoalGuide GetGoalGuide(Goal goal, ScenarioGuide scenarioGuide = null);

    ScenarioGuide GetScenarioGuide(int scenarioId);

    IEnumerable<ScenarioGuide> GetAllScenarioGuide();
  }
}
