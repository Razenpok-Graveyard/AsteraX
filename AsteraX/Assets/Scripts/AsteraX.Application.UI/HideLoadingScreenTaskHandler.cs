using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.UI
{
    public class HideLoadingScreenTaskHandler : AsyncApplicationTaskHandler<HideLoadingScreen>
    {
        [SerializeField] private LoadingScreen _loadingScreen;

        protected override UniTask Handle(HideLoadingScreen task, CancellationToken ct)
        {
            return _loadingScreen.Hide();
        }
    }
}