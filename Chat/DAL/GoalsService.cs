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

    public async Task<int> AchieveNextTrainingGoal(int trainingId)
    {
      try
      {
        _logger.LogInformation($"Achieving goal the next goal of training {trainingId}");
        var goal = await GetCurrentGoalByTrainingId(trainingId);
        goal.IsAchieved = true;
        _context.Update(goal);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Achieved goal order: {goal?.Goal?.Order}");
        return goal.Goal.Order;
      }
      catch (Exception ex)
      {
        _logger.LogError("An exception was thrown during achieving training goal: ", ex);
        return 0;
      }

    }

    public async Task CreateTrainingGoals(int trainingId, int scenarioId)
    {
      try
      {
        var goals = (from g in _context.Goals
                     where g.ScenarioId == scenarioId
                     select new TrainingGoal()
                     {
                       TrainingId = trainingId,
                       GoalId = g.Id,
                       IsAchieved = false
                     }).ForEachAsync(item => _context.Add(item));
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed creating all training goals: ", ex);
      }

    }

    public async Task<TrainingGoal> GetCurrentGoalBySessionid(int sessionId)
    {
      try
      {
        _logger.LogInformation($"Fetching the current goal from the db for session {sessionId}.");
        var result = await (from ts in _context.ChatSessions
                            join tg in _context.TrainingGoals on ts.TrainingId equals tg.TrainingId
                            where ts.Id == sessionId && !tg.IsAchieved
                            orderby tg.Goal.Order
                            select tg).Include(tg => tg.Goal).FirstOrDefaultAsync();
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
                            where tg.TrainingId == trainingId && !tg.IsAchieved
                            orderby tg.Goal.Order
                            select tg).Include(g => g.Goal).FirstOrDefaultAsync();
        _logger.LogInformation($"The current goal is {result}.");
        return result;
      }
      catch (Exception ex)
      {
        _logger.LogError("An exception was thrown: ", ex);
        return null;
      }

    }

    public async Task<int> GetTotalGoalsCount(int trainingId)
    {
      try
      {
        _logger.LogInformation($"Fetching the current goal from the db for training {trainingId}.");
        var result = await (from tg in _context.TrainingGoals
                            where tg.TrainingId == trainingId
                            select tg).CountAsync();
        _logger.LogInformation($"The current goal is {result}.");
        return result;
      }
      catch (Exception ex)
      {
        _logger.LogError("An exception was thrown: ", ex);
        return 0;
      }
    }

    public async Task UnAchieveAllTrainingGoals(int trainingId)
    {
      try
      {
        _logger.LogInformation($"UnAchieving all trainingGoals in the db for training {trainingId}.");
        await (from tg in _context.TrainingGoals
               where tg.TrainingId == trainingId && tg.IsAchieved
               select tg).ForEachAsync(item => item.IsAchieved = false);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"finished UnAchieving.");
      }
      catch (Exception ex)
      {
        _logger.LogError("An exception was thrown: ", ex);
      }

    }
  }


}
