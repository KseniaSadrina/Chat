using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Services
{
  public interface IGuideService
  {
    GoalGuide GetGoalGuide(Goal goal);
  }
}
