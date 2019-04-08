using Chat.DAL;
using Microsoft.AspNetCore.SignalR;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Chat.Hubs
{
  public class MessageHub: Hub
  {
    private readonly ChatContext _context;
    private readonly ISessionsService _sessionsService;

    public MessageHub(ChatContext context, ISessionsService sessionsService)
    {
      _context = context;
      _sessionsService = sessionsService;
    }

    // Add the user to the session, notify all session participants that the user has joined the room and save the session to the db
    public async Task AddToSession(int trainingId, int userId)
    {
      // first retrieve the first session (at the moment I support one session per training only)
      var session = (await _sessionsService.GetSessionsByTrainingId(trainingId)).FirstOrDefault();
      var sessionName = session.Name;
      Console.WriteLine($"Adding user to session: userid: {Context.ConnectionId}");
      await Groups.AddToGroupAsync(Context.ConnectionId, sessionName);
      await Clients.Client(Context.ConnectionId).SendAsync("groupJoin", session); // send notification to the subscriber
      await Clients.Group(sessionName).SendAsync("group", $"{Context.ConnectionId} has joined the group {sessionName}."); // send notification to the group

      await _sessionsService.AddUserToSession(session, userId);
    }

    // Remove the user from the group, send the group a notification and remove the mapping of the session and the user from the db
    public async Task RemoveFromSession(string sessionName, int userId)
    {
      await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionName);
      await Clients.Group(sessionName).SendAsync("group", $"{Context.ConnectionId} has left the group {sessionName}."); // send notification to the group
      await Clients.Client(Context.ConnectionId).SendAsync("groupLeave", sessionName);
      await _sessionsService.RemoveUserFromSession(sessionName, userId);
    }

    // send message to the whole group 
    public async Task SendToAll(Message message)
    {
      Console.WriteLine($"Sending message: {message?.Text} to group: {message?.Sender}");
      await Clients.Groups(message.SessionName).SendAsync("message", message);
      await _context.Messages.AddAsync(message);
    }

  }

}
