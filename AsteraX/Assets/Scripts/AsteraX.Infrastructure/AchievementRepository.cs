using System.Collections.Generic;
using System.Linq;
using AsteraX.Domain.Achievements;

namespace AsteraX.Infrastructure
{
    public class AchievementRepository
    {
        private readonly List<Achievement> _achievements = new List<Achievement>
        {
            new Achievement(1, AchievementGoalType.KilledAsteroidCount, 1, 0, "FIRST DUST", "Shot Your First Asteroid"),
            new Achievement(2, AchievementGoalType.LuckyShotCount, 1, 0, "LUCKY SHOT", "Bullet Wrapped Screen and Hit an Asteroid"),
            new Achievement(3, AchievementGoalType.ShotCount, 1000, 0, "TRIGGER HAPPY", "1,000 Shots Fired"),
            new Achievement(4, AchievementGoalType.HighScore, 10000, 0, "ROOKIE PILOT", "Score Above 10,000"),
            new Achievement(5, AchievementGoalType.LuckyShotCount, 100, 0, "EAGLE EYE", "100 Lucky Shots"),
            new Achievement(6, AchievementGoalType.LevelReached, 5, 0, "SKILLFUL DODGER", "Reach Level 5")
        };
        
        public IReadOnlyList<Achievement> GetNonAchievedByType(AchievementGoalType type)
        {
            return _achievements.Where(a => a.GoalType == type && !a.IsAchieved).ToList();
        }
    }
}