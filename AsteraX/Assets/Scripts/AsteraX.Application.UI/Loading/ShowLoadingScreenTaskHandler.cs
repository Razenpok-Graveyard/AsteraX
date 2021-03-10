using System.Threading;
using AsteraX.Application.Tasks.UI;
using Common.Application.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.UI.Loading
{
    public class ShowLoadingScreenTaskHandler : AsyncApplicationTaskHandler<ShowLoadingScreen>
    {
        [SerializeField] private LoadingScreen _loadingScreen;

        protected override UniTask Handle(ShowLoadingScreen task, CancellationToken ct)
        {
            return _loadingScreen.Show(task.Id, task.Asteroids, task.Children);
        }
    }
}