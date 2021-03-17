using System.Collections.Generic;
using System.Linq;
using AsteraX.Domain.Achievements;

namespace AsteraX.Infrastructure
{
    public class AchievementRepository
    {
        private readonly List<Achievement> _achievements = new List<Achievement>
        {
            new Achievement(0, AchievementGoalType.KilledAsteroidCount, 1, 0, "FIRST DUST", "Shot Your First Asteroid")
        };
        
        public IReadOnlyList<Achievement> GetNonAchievedByType(AchievementGoalType type)
        {
            return _achievements.Where(a => a.GoalType == type && !a.IsAchieved).ToList();
        }

        public void Save()
        {
        }
    }
}