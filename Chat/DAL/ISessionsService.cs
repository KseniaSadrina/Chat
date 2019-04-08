using Models;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.DAL
{
  public interface ISessionsService
  {

    Task<DbExecutionStatus> Add(ChatSession session);

    Task<DbExecutionStatus> Add(int sessionCreatorId, ChatSession session);

    Task<IEnumerable<ChatSession>> GetAllAsync();

    Task<IEnumerable<ChatSession>> GetSessionsByTrainingId(int trainingId);

    Task<IEnumerable<ChatSession>> GetUserSessions(int userId);

    Task<ChatSession> Get(int id);

    Task<DbExecutionStatus> AddUserToSession(ChatSession session, int userId);

    Task<DbExecutionStatus> RemoveUserFromSession(string sessionName, int userId);

  }
}
