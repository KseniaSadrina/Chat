using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.DAL
{
  public interface IGoalsService
  {
    Task<TrainingGoal> GetCurrentGoalByTrainingId(int trainingId);

    Task<TrainingGoal> GetCurrentGoalBySessionid(int sessionId);

    Task<int> GetTotalGoalsCount(int trainingId);

    Task<int> AchieveNextTrainingGoal(int trainingId);

    Task UnAchieveAllTrainingGoals(int trainingId);

    Task CreateTrainingGoals(int trainingId, int scenarioId);
  }
}
