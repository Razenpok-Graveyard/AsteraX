using System.Collections.Generic;
using System.Linq;
using AsteraX.Domain.Achievements;
using Razensoft.Functional;

namespace AsteraX.Infrastructure
{
    public class AchievementRepository
    {
        private readonly List<Achievement> _achievements = new List<Achievement>
        {
            new Achievement(1, AchievementGoalType.KilledAsteroidCount, 1, 0, "FIRST DUST",
                "Shot Your First Asteroid"),
            new Achievement(2, AchievementGoalType.LuckyShotCount, 1, 0, "LUCKY SHOT",
                "Bullet Wrapped Screen and Hit an Asteroid"),
            new Achievement(3, AchievementGoalType.ShotCount, 1000, 0, "TRIGGER HAPPY",
                "1,000 Shots Fired"),
            new Achievement(4, AchievementGoalType.HighScore, 10000, 0, "ROOKIE PILOT",
                "Score Above 10,000"),
            new Achievement(5, AchievementGoalType.LuckyShotCount, 100, 0, "EAGLE EYE",
                "100 Lucky Shots"),
            new Achievement(6, AchievementGoalType.LevelReached, 5, 0, "SKILLFUL DODGER",
                "Reach Level 5")
        };

        private readonly SaveFile _saveFile;

        public AchievementRepository(SaveFile saveFile)
        {
            _saveFile = saveFile;
        }

        public IReadOnlyList<Achievement> GetNonAchievedByType(AchievementGoalType type)
        {
            var saveFile = _saveFile.GetContents();
            return _achievements
                .Where(a => a.GoalType == type)
                .Where(a => !saveFile.Achievements.Contains(a.Id))
                .Select(a => CreateFromSaveFile(a, saveFile))
                .ToList();
        }

        private static Achievement CreateFromSaveFile(Achievement achievement, SaveFileContents saveFileContents)
        {
            var maybeProgress = saveFileContents.AchievementProgress.TryFirst(p => p.Id == achievement.Id);
            int progress;
            if (saveFileContents.Achievements.Contains(achievement.Id))
            {
                progress = achievement.Goal;
            }
            else if (maybeProgress.HasValue)
            {
                progress = maybeProgress.Value.Progress;
            }
            else
            {
                progress = 0;
            }

            return new Achievement(
                achievement.Id,
                achievement.GoalType,
                achievement.Goal,
                progress,
                achievement.Name,
                achievement.Description
            );
        }

        public void Save(IReadOnlyList<Achievement> achievements)
        {
            var saveFile = _saveFile.GetContents();
            foreach (var achievement in achievements)
            {
                UpdateSave(saveFile, achievement);
            }
            _saveFile.SaveContents(saveFile);
        }

        private static void UpdateSave(SaveFileContents saveFileContents, Achievement achievement)
        {
            var maybeProgress = saveFileContents.AchievementProgress.TryFirst(p => p.Id == achievement.Id);
            if (achievement.IsAchieved)
            {
                if (maybeProgress.HasValue)
                {
                    saveFileContents.AchievementProgress.Remove(maybeProgress.Value);
                }

                if (!saveFileContents.Achievements.Contains(achievement.Id))
                {
                    saveFileContents.Achievements.Add(achievement.Id);
                }

                return;
            }

            AchievementProgress progress;
            if (maybeProgress.HasNoValue)
            {
                progress = new AchievementProgress
                {
                    Id = achievement.Id
                };
                saveFileContents.AchievementProgress.Add(progress);
            }
            else
            {
                progress = maybeProgress.Value;
            }

            progress.Progress = achievement.Progress;
        }
    }
}