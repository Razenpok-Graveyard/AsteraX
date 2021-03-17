using System.Threading;
using AsteraX.Application.UI.Requests;
using Common.Application.Unity;
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

        private void Awake()
        {
            this.RegisterHandler<ShowGameOverScreen>(Handle);
            this.RegisterHandler<HideGameOverScreen>(Handle);
        }

        private async UniTask Handle(ShowGameOverScreen task, CancellationToken ct)
        {
            _level.text = $"Final level: {task.Level}";
            _score.text = $"Final score: {task.Score}";
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 0;
            await _canvasGroup.DOFade(1, 0.5f)
                .SetUpdate(true)
                .WithCancellation(ct);
        }

        private async UniTask Handle(HideGameOverScreen task, CancellationToken ct)
        {
            await UniTask.Delay(3000, cancellationToken: ct);
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0;
        }
    }
}