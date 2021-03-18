using System;
using System.Collections.Generic;
using AsteraX.Application.Game.Notifications;
using AsteraX.Application.UI.Requests;
using AsteraX.Domain.Achievements;
using AsteraX.Infrastructure;
using Common.Application;

namespace AsteraX.Application.Achievements
{
    public class AchievementController : IDisposable
    {
        private readonly List<IDisposable> _registrations = new List<IDisposable>();

        private readonly AchievementRepository _achievementRepository;
        private readonly OutputMediator _mediator;

        public AchievementController(
            AchievementRepository achievementRepository,
            OutputMediator mediator)
        {
            _achievementRepository = achievementRepository;
            _mediator = mediator;
            Register<AsteroidShot>(Handle);
            Register<HighScoreUpdated>(Handle);
            Register<ShotFired>(Handle);
            Register<LevelReached>(Handle);
        }

        private void Handle(AsteroidShot notification)
        {
            UpdateProgress(AchievementGoalType.KilledAsteroidCount, progress => progress + 1);
        }

        private void Handle(HighScoreUpdated notification)
        {
            UpdateProgress(AchievementGoalType.HighScore, _ => notification.Score);
        }

        private void Handle(ShotFired notification)
        {
            UpdateProgress(AchievementGoalType.ShotCount, progress => progress + 1);
        }

        private void Handle(LevelReached notification)
        {
            UpdateProgress(AchievementGoalType.LevelReached, _ => (int) notification.Id);
        }

        private void UpdateProgress(AchievementGoalType type, Func<int, int> getNewProgress)
        {
            var achievements = _achievementRepository.GetNonAchievedByType(type);
            foreach (var achievement in achievements)
            {
                var newProgress = getNewProgress(achievement.Progress);
                achievement.UpdateProgress(newProgress);
                if (achievement.IsAchieved)
                {
                    _mediator.ForgetSend(new ShowAchievementPopup
                    {
                        Title = achievement.Name,
                        Description = achievement.Description
                    });
                }
            }
        }

        public void Dispose()
        {
            foreach (var registration in _registrations)
            {
                registration.Dispose();
            }
            _registrations.Clear();
        }

        private void Register<TNotification>(Action<TNotification> handler)
            where TNotification : INotification
        {
            var registration = _mediator.RegisterNotificationHandler(handler);
            _registrations.Add(registration);
        }
    }
}