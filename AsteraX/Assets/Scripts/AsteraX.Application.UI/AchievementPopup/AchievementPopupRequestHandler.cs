using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AsteraX.Application.UI.Requests;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Razensoft.Mediator;
using TMPro;
using UniRx;
using UnityEngine;
using Unit = UniRx.Unit;

namespace AsteraX.Application.UI.AchievementPopup
{
    public class AchievementPopupRequestHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;

        private Queue<IAsyncRequest> _queue = new Queue<IAsyncRequest>();
        private readonly Subject<Unit> _completed = new Subject<Unit>();

        private void Awake()
        {
            this.RegisterHandler<ShowAchievementPopup>(Handle);
            this.RegisterHandler<ShowHighScorePopup>(Handle);
        }

        private async UniTask Handle(ShowAchievementPopup request, CancellationToken ct)
        {
            _queue.Enqueue(request);
            await WaitInQueue(request, ct);
            await ShowAchievementPopupAsync(request.Title, request.Description, ct);
            _queue.Dequeue();
            _completed.OnNext(Unit.Default);
        }

        private async UniTask Handle(ShowHighScorePopup request, CancellationToken ct)
        {
            _queue.Enqueue(request);
            await WaitInQueue(request, ct);
            await ShowAchievementPopupAsync("High Score!", "You've achieved a new high score.", ct);
            _queue.Dequeue();
            _completed.OnNext(Unit.Default);
        }

        private async UniTask ShowAchievementPopupAsync(string title, string description, CancellationToken ct)
        {
            _title.text = title;
            _description.text = description;

            await _root.DOAnchorPosY(-25, 0.5f).WithCancellation(ct);
            await UniTask.Delay(1000, cancellationToken: ct);
            await _root.DOAnchorPosY(175, 0.5f).WithCancellation(ct);
        }

        private UniTask WaitInQueue(IAsyncRequest request, CancellationToken ct)
        {
            if (_queue.Count == 1)
            {
                return UniTask.CompletedTask;
            }

            var tcs = new UniTaskCompletionSource();
            var registration = ct.Register(() =>
            {
                _queue = new Queue<IAsyncRequest>(_queue.Where(r => r != request));
                tcs.TrySetCanceled();
            });
            _completed.Where(_ => _queue.Peek() == request)
                .First()
                .Subscribe(_ =>
                {
                    registration.Dispose();
                    tcs.TrySetResult();
                });
            return tcs.Task;
        }
    }
}