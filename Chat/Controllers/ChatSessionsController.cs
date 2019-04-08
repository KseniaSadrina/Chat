using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Chat.DAL;
using Models;
using Chat.Helpers;
using System.Linq;
using System;

namespace Chat.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ChatSessionsController : ControllerBase
  {
    private readonly ISessionsService _sessionsService;

    public ChatSessionsController(ISessionsService sessionService)
    {
      _sessionsService = sessionService;
    }

    // GET: api/ChatSessions
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatSession>>> GetChannels()
    {
      var result = await _sessionsService.GetAllAsync();
      return new JsonResult(result);
    }


    // GET: api/ChatSessions
    [HttpGet("{id}")]
    public async Task<ActionResult<ChatSession>> GetChatSession(int id)
    {
      var result = await _sessionsService.Get(id);
      return new JsonResult(result);
    }

    // GET: api/ChatSessions?userId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatSession>>> GetUserSessions(int userId)
    {
      // at the moment we are retrieving only one session in such case
      var result = await _sessionsService.GetUserSessions(userId);
      Console.WriteLine($"result of getting sessions: {result}");
      return new JsonResult(result);
    }

    // GET: api/ChatSessions?userId=1&trainingId=2
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatSession>>> GetUserSessionsByTrainingId(int userId, int trainingId)
    {
      // at the moment we are retrieving only one session in such case
      var result = await _sessionsService.GetUserSessions(userId);
      return new JsonResult(result);
    }

    // POST: api/ChatSessions
    [HttpPost]
    public async Task<ActionResult<ChatSession>> PostChatSession(ChatSession chatSession)
    {
      var result = await _sessionsService.Add(chatSession);
      return result.ConvertToWebAPI<ChatSession>(CreatedAtAction("GetChatSession", new { id = chatSession.Id }, chatSession));
    }


  }
}
