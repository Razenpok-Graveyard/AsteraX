using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace AsteraX.Application.UI.GameOver
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private TextMeshProUGUI _score;

        public async UniTask ShowAsync(int level, int score)
        {
            _level.text = $"Final level: {level}";
            _score.text = $"Final score: {score}";
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 0;
            await _canvasGroup.DOFade(1, 0.5f).SetUpdate(true);
        }
    }
}