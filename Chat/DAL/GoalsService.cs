using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;

namespace Chat.DAL
{
  public class GoalsService : IGoalsService
  {
    private readonly ChatContext _context;
    private readonly ILogger<GoalsService> _logger;

    public GoalsService(ChatContext context, ILogger<GoalsService> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task<TrainingGoal> GetCurrentGoalBySessionid(int sessionId)
    {
      try
      {
        _logger.LogInformation($"Fetching the current goal from the db for session {sessionId}.");
        var result = await(from ts in _context.ChatSessions
                           join tg in _context.TrainingGoals on ts.TrainingId equals tg.TrainingId
                           where ts.Id == sessionId && !tg.IsAchieved
                           orderby tg.Goal.Order
                           select tg).FirstOrDefaultAsync();
        _logger.LogInformation($"The current goal is {result}.");
        return result;
      }
      catch (Exception ex)
      {
        _logger.LogError("An exception was thrown: ", ex);
        return null;
      }
    }

    public async Task<TrainingGoal> GetCurrentGoalByTrainingId(int trainingId)
    {
      try
      {
        _logger.LogInformation($"Fetching the current goal from the db for training {trainingId}.");
        var result = await (from tg in _context.TrainingGoals
                            where tg.TrainingId == trainingId  && !tg.IsAchieved
                            orderby tg.Goal.Order
                            select tg).FirstOrDefaultAsync();
        _logger.LogInformation($"The current goal is {result}.");
        return result;
      }
      catch (Exception ex)
      {
        _logger.LogError("An exception was thrown: ", ex);
        return null;
      }
      
    }
  }
}
