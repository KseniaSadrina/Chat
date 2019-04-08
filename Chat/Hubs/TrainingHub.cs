using Chat.DAL;
using Microsoft.AspNetCore.SignalR;
using Models;
using Models.Enums;
using System;
using System.Threading.Tasks;

namespace Chat.Hubs
{
  public class TrainingHub: Hub
  {

    private readonly ITrainingService _trainingService;

    public TrainingHub(ITrainingService trainingService)
    {
      _trainingService = trainingService;
    }

    public async Task NotifyAddAll(int trainingCreatorId, Training training)
    {
      Console.WriteLine($"Received training: {training.Name}, {training.Scenario?.Id}, {training?.State}, {training.Scenario?.Name}");
      var result = await _trainingService.Add(trainingCreatorId, training);

      // If the addition has succeeded, publish to everybody else that there is a new active training available
      if (result == DbExecutionStatus.Succeeded)
        await Clients.All.SendAsync("add", training);
    }

    public async Task NotifyUpdateAll(int id, Training training)
    {
      if (id != training.Id) return;

      var result = await _trainingService.Update(id, training);

      // If the edit has succeeded, publish to everybody else that there is a a change in a training
      if (result == DbExecutionStatus.Succeeded)
        await Clients.All.SendAsync("update", id, training);
    }

  }
}
