using Chat.DAL;
using Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Chat.Hubs
{
  [Authorize]
  public class MessageHub: Hub
  {

    #region Private Fields

    private readonly ChatContext _context;
    private readonly ISessionsService _sessionsService;
    private readonly ILogger<MessageHub> _logger;
    private readonly IBotService _botService;

    #endregion

    #region Init

    public MessageHub(ChatContext context,
      ISessionsService sessionsService,
      ILogger<MessageHub> logger,
      IBotService botService)
    {
      _context = context;
      _sessionsService = sessionsService;
      _logger = logger;
      _botService = botService;

    }

    #endregion

    #region Session/Group Methods

    // Add the user to the session, notify all session participants that the user has joined the room and save the session to the db
    public async Task AddToSession(int trainingId, int userId)
    {
      // first retrieve the first session (at the moment I support one session per training only)
      var session = (await _sessionsService.GetSessionsByTrainingId(trainingId)).FirstOrDefault();
      var sessionName = session.Name;
      _logger.LogInformation($"Adding user to session: userid: {Context.ConnectionId}");
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

    #endregion

    #region Messages Methods

    // send message to the whole group 
    public async Task SendToAll(Message message)
    {
      _logger.LogInformation($"Sending message: {message?.Text} to group: {message?.SessionName}");
      await Clients.Groups(message.SessionName).SendAsync("message", message);

      await _context.AddAsync(message);
      await _context.SaveChangesAsync();
      await SendToBot(message);
    }

    private async Task SendToBot(Message message)
    {
      _logger.LogDebug($"Let Marley deal with the message.");
      if (_botService.CurrentBot == null) return;

      var (willHandle, handlerType) = _botService.WillHandleMessage(message);
      if (!willHandle) return;

      _logger.LogDebug($"Marley decided to take care of this one (probable was mentioned there).");
      await StartTyping(message.SessionName, _botService.CurrentBot.UserName);
      var response = await _botService.HandleMessage(message, handlerType);

      // stop typing and send the message to the session participents
      await StopTyping(message.SessionName, _botService.CurrentBot.UserName);

      if (response == null) return;

      _logger.LogInformation($"Sending message: {response?.Text} to group: {message?.SessionName}");
      await Clients.Groups(message.SessionName).SendAsync("message", response);

      // save marley's message in the db
      await _context.AddAsync(response);
      await _context.SaveChangesAsync();
    }

    #endregion

    #region Typing indication

    public async Task StartTyping(string sessionName, string userName) => await UserTyping(sessionName, userName);

    public async Task StopTyping(string sessionName, string userName) => await UserTyping(sessionName, userName, true);

    public async Task UserTyping(string sessionName, string userName, bool stopped=false)
    {
      _logger.LogInformation($"User started/stopped typing: {userName}, stopped: {stopped}");

      if (!stopped)
        await Clients.Groups(sessionName).SendAsync("startTyping", sessionName, userName);
      else await Clients.Groups(sessionName).SendAsync("stoppedTyping", sessionName);

    }

    #endregion
  }

}
