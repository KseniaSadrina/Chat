using Models;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.DAL
{
  public interface ITrainingDALService
  {
    Task<DbExecutionStatus> Add(Training training);

    Task<DbExecutionStatus> Add(int trainingCreatorId, Training training);

    Task<IEnumerable<Training>> GetAll();

    Task<DbExecutionStatus> Delete(int id);

    Task<IEnumerable<ChatSession>> GetTrainingSessions(int id);

    Task<Training> Get(int trainingId);

    Task<DbExecutionStatus> Update(int id, Training updatedTraining);

    Task<Training> GetTrainingBySessionId(int sessionId);

  }
}
