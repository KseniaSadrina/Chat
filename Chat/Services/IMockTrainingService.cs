using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Services
{
  public interface IMockTrainingService
  {

    Task<bool> StartMockTraining(int trainingId);

    IDictionary<int, int> GetAllCurrentMocks();

  }
}
