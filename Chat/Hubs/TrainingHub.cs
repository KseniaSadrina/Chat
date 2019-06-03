using Chat.Configuration;
using Chat.DAL;
using Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Models;
using Models.Enums;
using System;
using System.Threading.Tasks;

namespace Chat.Hubs
{
  [Authorize]
  public class TrainingHub: Hub
  {
    private readonly ITrainingDALService _trainingDALService;

    public TrainingHub(ITrainingDALService trainingService)
    {
      _trainingDALService = trainingService;
    }

    public async Task NotifyAddAll(int trainingCreatorId, Training training)
    {
      Console.WriteLine($"Received training: {training.Name}, {training.Scenario?.Id}, {training?.State}, {training.Scenario?.Name}");
      var result = await _trainingDALService.Add(trainingCreatorId, training);

      // If the addition has succeeded, publish to everybody else that there is a new active training available
      if (result == DbExecutionStatus.Succeeded)
        await Clients.All.SendAsync("add", training);
    }

    public async Task NotifyUpdateAll(int id, Training training)
    {
      if (id != training.Id) return;

      var result = await _trainingDALService.Update(id, training);

      // If the edit has succeeded, publish to everybody else that there is a a change in a training
      if (result == DbExecutionStatus.Succeeded)
        await Clients.All.SendAsync("update", id, training);
    }

  }
}
