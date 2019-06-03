using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace Chat.DAL
{
  public class TrainingDALService : ITrainingDALService
  {
    private readonly ChatContext _context;
    private readonly ISessionsService _sessionsService;
    private readonly IGoalsService _goalsService;

    public TrainingDALService(ChatContext context,
                              ISessionsService sessionsService,
                              IGoalsService goals)
    {
      _context = context;
      _sessionsService = sessionsService;
      _goalsService = goals;
    }

    public async Task<DbExecutionStatus> Add(Training training)
    {
      try
      {
        // use the following statement so that City won't be inserted
        var newTraining = await _context.Trainings.AddAsync(training);
        _context.Entry(training.Scenario).State = EntityState.Unchanged;
        await _context.SaveChangesAsync();
        return DbExecutionStatus.Succeeded;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Failed adding training, exception: {ex}");
        return DbExecutionStatus.Failed;
      }
     
    }

    public async Task<DbExecutionStatus> Add(int trainingCreatorId, Training training)
    {
      try
      {
        var result = await Add(training);
        if (result == DbExecutionStatus.Failed) return result;

        // Now create session for this training
        var newSession = new ChatSession()
        {
          Type = ChannelType.Private,
          TrainingId = training.Id,
          Name = training.Name + new Random().Next()
        };

        // Create training goals for this training
        await _goalsService.CreateTrainingGoals(training.Id, training.Scenario.Id);


        return await _sessionsService.Add(newSession);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Failed Saving the training {ex}");
        return DbExecutionStatus.Failed;
      }
    }

    public async Task<DbExecutionStatus> Delete(int id)
    {
      var training = await _context.Trainings.FindAsync(id);
      if (training == null)
      {
        return DbExecutionStatus.NotFound;
      }

      _context.Trainings.Remove(training);
      await _context.SaveChangesAsync();

      return DbExecutionStatus.Succeeded;
    }

    public async Task<Training> Get(int trainingId)
    {
      return await _context.Trainings.FindAsync(trainingId);
    }

    public async Task<IEnumerable<Training>> GetAll()
    {
      return await _context.Trainings
        .Include(t => t.Scenario)
        .ToListAsync();
    }

    public async Task<Training> GetTrainingBySessionId(int sessionId)
    {
      return await (from t in _context.Trainings
       join s in _context.ChatSessions on t.Id equals s.TrainingId
       where s.Id == sessionId
       select t).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ChatSession>> GetTrainingSessions(int id)
    {
      return await _context.ChatSessions
        .Where(session => session.TrainingId == id).ToListAsync();
    }

    public async Task<DbExecutionStatus> Update(int id, Training updatedTraining)
    {
      _context.Entry(updatedTraining).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException ex)
      {
        if (!TrainingExists(id))
        {
          return DbExecutionStatus.NotFound;
        }
        else
        {
          Console.WriteLine($"Failed inserting new training: {ex}");
          return DbExecutionStatus.Failed;
        }
      }

      return DbExecutionStatus.Succeeded;

    }

    private bool TrainingExists(int id)
    {
      return _context.Trainings.Any(e => e.Id == id);
    }
  }
}
