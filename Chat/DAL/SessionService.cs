using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Helpers;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace Chat.DAL
{
  public class SessionService : ISessionsService
  {
    private readonly ChatContext _context;

    public SessionService(ChatContext context)
    {
      _context = context;
    }

    public async Task<DbExecutionStatus> Add(ChatSession session)
    {
      try
      {
        _context.ChatSessions.Add(session);
        await _context.SaveChangesAsync();
        return DbExecutionStatus.Succeeded;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Failed creating new session {ex}");
        return DbExecutionStatus.Failed;
      }

    }

    public async Task<DbExecutionStatus> Add(int sessionCreatorId, ChatSession session)
    {
      var result = await Add(session);
      if (result != DbExecutionStatus.Succeeded) return result;

      try
      {
        // Since we have many to many relationship, we need to perform mapping between those entities
        _context.SessionsUsers.Add(new SessionUser() { SessionId = session.Id, UserId = Consts.MARLEYID }); // add Marley automatically
        _context.SessionsUsers.Add(new SessionUser() { SessionId = session.Id, UserId = sessionCreatorId });

        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Failed creating new mapping between session and user {ex}");
        return DbExecutionStatus.Failed;
      }

      return DbExecutionStatus.Succeeded;
    }

    public async Task<DbExecutionStatus> AddUserToSession(ChatSession session, int userId)
    {
      if (session == null) return DbExecutionStatus.NotFound;

      // check if this user is alredy registered in this session
      var mapping = _context.SessionsUsers.Where(us => us.SessionId == session.Id && us.UserId == userId).FirstOrDefault();
      if (mapping != null) return DbExecutionStatus.Succeeded;

      // else, add mapping
      _context.SessionsUsers.Add(new SessionUser() { UserId = userId, SessionId = session.Id });
      await _context.SaveChangesAsync();

      return DbExecutionStatus.Succeeded;
    }

    public async Task<ChatSession> Get(int id)
    {
      return await _context.ChatSessions.FindAsync(id);
    }

    public async Task<IEnumerable<ChatSession>> GetAllAsync()
    {
      return await _context.ChatSessions.ToListAsync();
    }

    public async Task<IEnumerable<ChatSession>> GetSessionsByTrainingId(int trainingId)
    {
      return await _context.ChatSessions
        .Where(s => s.TrainingId == trainingId)
        .Include(s => s.Messages)
        .ToListAsync();
    }

    public async Task<IEnumerable<ChatSession>> GetUserSessions(int userId, int trainingId)
    {
      var result = await _context.Users
        .Where(u => u.Id == userId)
        .SelectMany(s => s.Sessions.Where(us => us.UserId == userId && us.Session.TrainingId == trainingId))
        .Select(us => us.Session)
        .Distinct()
        .ToListAsync();

      return result;

    }

    public async Task<IEnumerable<ChatSession>> GetUserSessions(int userId)
    {
      var result = await _context.ChatSessions.SelectMany(s => s.Users
      .Where(us => us.UserId == userId))
      .Select(us => us.Session)
      .Distinct()
      .ToListAsync();

      return result;

    }

    public async Task<DbExecutionStatus> RemoveUserFromSession(string sessionName, int userId)
    {
      var session = await _context.ChatSessions.FirstOrDefaultAsync(s => s.Name == sessionName);
      if (session == null) return DbExecutionStatus.NotFound;

      var userSession = await _context.SessionsUsers.FirstOrDefaultAsync(s => s.UserId == userId && s.SessionId == session.Id);
      if (userSession == null) return DbExecutionStatus.NotFound;

      _context.SessionsUsers.Remove(userSession);
      await _context.SaveChangesAsync();

      return DbExecutionStatus.Succeeded;
    }
  }
}
