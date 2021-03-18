using System;
using Razensoft.Domain;

namespace AsteraX.Domain.Achievements
{
    public class Achievement : Entity
    {
        public Achievement(
            long id,
            AchievementGoalType goalType,
            int goal,
            int progress,
            string name,
            string description)
            : base(id)
        {
            GoalType = goalType;
            Goal = goal;
            Progress = progress;
            Name = name;
            Description = description;
        }
        
        public AchievementGoalType GoalType { get; }
        
        public int Goal { get; }
        
        public int Progress { get; private set; }

        public bool IsAchieved => Progress >= Goal;
        
        public string Name { get; }
        
        public string Description { get; }

        public void UpdateProgress(int progress) => Progress = Math.Max(Progress, progress);
    }
}