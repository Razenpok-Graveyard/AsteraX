using System.Threading;
using AsteraX.Application.UI.Requests;
using Common.Application.Unity;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace AsteraX.Application.UI.Loading
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _screenCanvasGroup;
        [SerializeField] private CanvasGroup _footerCanvasGroup;

        [SerializeField] private TextMeshProUGUI _idLabel;
        [SerializeField] private TextMeshProUGUI _asteroidsLabel;
        [SerializeField] private TextMeshProUGUI _childrenLabel;

        private void Awake()
        {
            this.RegisterHandler<ShowLoadingScreen>(Handle);
            this.RegisterHandler<HideLoadingScreen>(Handle);
        }

        private UniTask Handle(ShowLoadingScreen task, CancellationToken ct)
        {
            _idLabel.text = $"Level {task.Id}";
            _asteroidsLabel.text = $"Asteroids: {task.Asteroids}";
            _childrenLabel.text = $"Children: {task.Children}";
            return FadeIn(ct);
        }

        private UniTask Handle(HideLoadingScreen task, CancellationToken ct)
        {
            return FadeOut(ct);
        }

        private async UniTask FadeIn(CancellationToken ct)
        {
            _footerCanvasGroup.alpha = 0;
            _screenCanvasGroup.alpha = 0;
            _screenCanvasGroup.blocksRaycasts = true;
            await _screenCanvasGroup.DOFade(1, 0.5f).WithCancellation(ct);
            await _footerCanvasGroup.DOFade(1, 0.5f).WithCancellation(ct);
        }

        private async UniTask FadeOut(CancellationToken ct)
        {
            await UniTask.Delay(1000, DelayType.UnscaledDeltaTime, cancellationToken: ct);
            _footerCanvasGroup.alpha = 1;
            _screenCanvasGroup.alpha = 1;
            await _footerCanvasGroup.DOFade(0, 0.5f).WithCancellation(ct);
            await _screenCanvasGroup.DOFade(0, 0.5f).WithCancellation(ct);
            _screenCanvasGroup.blocksRaycasts = false;
        }
    }
}