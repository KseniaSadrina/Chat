using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class TrainingMock: ICloneable
    {
        public DateTime EndTime { get; set; }

        public int Progress { get; set; }

        public int TotalGoals { get; set; }

        public System.Timers.Timer TrainingTimer { get; set; }

        public int AchievedGoals { get; set; }

        public object Clone()
        {
            return new TrainingMock() { EndTime = EndTime, Progress = Progress, TotalGoals = TotalGoals, TrainingTimer = TrainingTimer, AchievedGoals = AchievedGoals };
        }
    }
}
