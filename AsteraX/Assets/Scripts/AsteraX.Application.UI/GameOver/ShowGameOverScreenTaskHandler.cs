using System.Threading;
using AsteraX.Application.Tasks.UI;
using Common.Application.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.UI.GameOver
{
    public class ShowGameOverScreenTaskHandler : AsyncApplicationTaskHandler<ShowGameOverScreen>
    {
        [SerializeField] private GameOverScreen _gameOverScreen;

        protected override UniTask Handle(ShowGameOverScreen task, CancellationToken ct)
        {
            return _gameOverScreen.ShowAsync(task.Level, task.Score);
        }
    }
}