using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Chat.DAL;
using Models;
using Chat.Hubs;
using Microsoft.AspNetCore.SignalR;
using Chat.Helpers;
using Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Chat.Services;

namespace Chat.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class TrainingsController : ControllerBase
  {
    private readonly ITrainingService _trainingService;
    private readonly IHubContext<TrainingHub> _hubContext;
    private readonly IMockTrainingService _mockService;
    

    public TrainingsController(ITrainingService trainingService, IMockTrainingService mock,  IHubContext<TrainingHub> hubContext)
    {
      _trainingService = trainingService;
      _hubContext = hubContext;
      _mockService = mock;
    }

    // GET: api/Trainings
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Training>>> GetTrainings()
    {
      var result = await _trainingService.GetAll();
      return new JsonResult(result);

    }

    // GET: api/Trainings/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Training>> GetTraining(int id)
    {
      var training = await _trainingService.Get(id);

      if (training == null)
      {
        return NotFound();
      }

      return training;
    }

    // GET: api/Trainings/5/Sessions
    [HttpGet("{id}/sessions")]
    public async Task<ActionResult<IEnumerable<ChatSession>>> GetTrainingSessions(int id)
    {
      var sessions = await _trainingService.GetTrainingSessions(id);

      if (sessions == null)
      {
        return NotFound();
      }

      return new JsonResult(sessions);
    }

    // PUT: api/Trainings/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTraining(int id, Training training)
    {
      if (id != training.Id)
      {
        return BadRequest();
      }

      var result = await _trainingService.Update(id, training);

      // If the edit has succeeded, publish to everybody else that there is a a change in a training
      if (result == DbExecutionStatus.Succeeded)
        await _hubContext.Clients.All.SendAsync("update", training);

      return result.ConvertToWebAPI(NoContent());
    }

    // POST: api/Trainings
    [HttpPost]
    public async Task<ActionResult<Training>> PostTraining(Training training)
    {
      var result = await _trainingService.Add(training);

      // If the addition has succeeded, publish to everybody else that there is a new active training available
      if (result == DbExecutionStatus.Succeeded)
        await _hubContext.Clients.All.SendAsync("add", training);

      return result.ConvertToWebAPI<Training>(CreatedAtAction("GetTraining", new { id = training.Id }, training));
    }

    [HttpPost("mocks/{id}")]
    public async Task<ActionResult> MockTraining(int id)
    {
      if (id < 0) return BadRequest();
      var result = await _mockService.StartMockTraining(id);
      return result ? Ok() : StatusCode(500);
    }

    [HttpGet("mocks")]
    public ActionResult GetMocks()
    {
      return Ok(_mockService.GetAllCurrentMocks());
    }

  }
}
