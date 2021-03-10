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

        public UniTask Show(int id, int asteroids, int children)
        {
            _idLabel.text = $"Level {id}";
            _asteroidsLabel.text = $"Asteroids: {asteroids}";
            _childrenLabel.text = $"Children: {children}";
            return FadeIn();
        }
        
        private async UniTask FadeIn() {

            _footerCanvasGroup.alpha = 0;
            _screenCanvasGroup.alpha = 0;
            _screenCanvasGroup.blocksRaycasts = true;
            await _screenCanvasGroup.DOFade(1, 0.5f).SetUpdate(true);
            await _footerCanvasGroup.DOFade(1, 0.5f).SetUpdate(true);
        }

        public UniTask Hide()
        {
            return FadeOut();
        }
        
        private async UniTask FadeOut()
        {
            await UniTask.Delay(1000, DelayType.UnscaledDeltaTime);
            _footerCanvasGroup.alpha = 1;
            _screenCanvasGroup.alpha = 1;
            await _footerCanvasGroup.DOFade(0, 0.5f).SetUpdate(true);
            await _screenCanvasGroup.DOFade(0, 0.5f).SetUpdate(true);
            _screenCanvasGroup.blocksRaycasts = false;
        }
    }
}