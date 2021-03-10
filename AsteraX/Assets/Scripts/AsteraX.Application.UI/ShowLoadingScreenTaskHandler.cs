using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.UI
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