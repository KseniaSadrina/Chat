using Chat.Configuration;
using Chat.DAL;
using Chat.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Chat.Services
{
  public class MockTrainingService : IMockTrainingService
  {
    private readonly IGoalsService _goalsService;
    // TODO: move this part to the cache
    // this dict contains the id's of the currently mocked trainings and their progress
    private ConcurrentDictionary<int, TrainingMock> _mockedTrainings = new ConcurrentDictionary<int, TrainingMock>();
    private TrainingMockConfiguration _config;
    private IHubContext<TrainingHub> _hubContext;
    private ILogger<MockTrainingService> _logger;

    public MockTrainingService(IGoalsService goals,
                               TrainingMockConfiguration config,
                               IHubContext<TrainingHub> hubContext,
                               ILogger<MockTrainingService> logger)
    {
      _goalsService = goals;
      _config = config;
      _hubContext = hubContext;
      _logger = logger;
    }

    public async Task<bool> StartMockTraining(int trainingId)
    {
      // count total goals to achieve
      var totalGoals = await _goalsService.GetTotalGoalsCount(trainingId);
      // create the task
      var tokenSource = new CancellationTokenSource();
      var token = tokenSource.Token;
      var task = new Task(() => MockTraining(trainingId, tokenSource), token, TaskCreationOptions.LongRunning);

      var mock = new TrainingMock() { EndTime = DateTime.Now + _config.Duration, Progress = 0, TotalGoals = totalGoals, AchievedGoals = 0 };
      var didSucceeded = _mockedTrainings.TryAdd(trainingId, mock);

      // check if the the mock isn't already running
      if (didSucceeded) task.Start();
      else if (task != null) task.Dispose();

      return didSucceeded;

    }

    private void MockTraining(int trainingId, CancellationTokenSource token)
    {
      var trainingTimer = new System.Timers.Timer(_config.TickInterval);
      trainingTimer.Elapsed += (sender, e) => OnTick(trainingId, token);

      if (_mockedTrainings.TryGetValue(trainingId, out var current))
      {
        var withTimer = current.Clone() as TrainingMock;
        withTimer.TrainingTimer = trainingTimer;
        if (_mockedTrainings.TryUpdate(trainingId, withTimer, current)) trainingTimer.Start();
        else trainingTimer.Dispose();
      }
    }

    private async void OnMockStop(int trainingId, CancellationTokenSource token)
    {
      await _goalsService.UnAchieveAllTrainingGoals(trainingId);

      _mockedTrainings.TryRemove(trainingId, out var mock);

      if (!token.IsCancellationRequested)
        token.Cancel();

      await _hubContext.Clients.All.SendAsync("finishedMock", trainingId);
    }

    private async void OnTick(int trainingId, CancellationTokenSource token)
    {
      if (_mockedTrainings.TryGetValue(trainingId, out var trainingMock))
      {
        if (DateTime.Now < trainingMock.EndTime)
        {
          trainingMock.Progress = CalculateRemainingProgress(trainingMock.EndTime);
          if (ShouldAchiveNext(trainingMock))
          {
            
            var achieved = await _goalsService.AchieveNextTrainingGoal(trainingId);
            trainingMock.AchievedGoals = achieved;
          }
          await _hubContext.Clients.All.SendAsync("progress", trainingId, trainingMock.Progress, trainingMock.AchievedGoals);
        }
        else
        {
          trainingMock.TrainingTimer.Stop();
          trainingMock.TrainingTimer.Dispose();
          OnMockStop(trainingId, token);
        }
      }

    }

    private bool ShouldAchiveNext(TrainingMock trainingMock)
    {
      // calculate the goal that should be marked as achieved now and return true/false
      var expected = ((float)trainingMock.Progress / 100) * trainingMock.TotalGoals;
      _logger.LogInformation($"progress: {trainingMock.Progress}; total goals: {trainingMock.TotalGoals} expected: {expected}");
      var result = expected >= trainingMock.AchievedGoals;
      _logger.LogInformation($"should achieve next: {result}");
      return result;
    }

    private int CalculateRemainingProgress(DateTime end)
    {
      var current = (end - DateTime.Now).TotalSeconds; // the time that has passed
      var total = _config.Duration.TotalSeconds;
      var remaining = (100 - (int)Math.Round((current * 100) / total));
      _logger.LogInformation($"The time theat has passed: {current} remaining time: {remaining} total: {total}");
      return remaining;
    }

    public IDictionary<int, int> GetAllCurrentMocks()
    {
      var result = new Dictionary<int, int>();
      foreach (var mock in _mockedTrainings.Keys)
        result[mock] = _mockedTrainings[mock].Progress;
      return result;
    }
  }
}
