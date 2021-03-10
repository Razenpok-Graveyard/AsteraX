using System.Threading;
using AsteraX.Application.Tasks.UI;
using Common.Application.Unity;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.UI.Loading
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