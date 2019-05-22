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
  }
}
