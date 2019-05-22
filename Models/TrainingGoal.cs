
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class TrainingGoal
    {
        public int GoalId { get; set; }

        public virtual Goal Goal { get; set; }

        [ForeignKey(nameof(Training))]
        public int TrainingId { get; set; }

        public bool IsAchieved { get; set; }

    }
}
