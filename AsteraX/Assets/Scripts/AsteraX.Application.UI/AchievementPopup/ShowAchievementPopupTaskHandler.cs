using System.Collections.Generic;
using System.Threading;
using AsteraX.Application.UI.Requests;
using Common.Application.Unity;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

namespace AsteraX.Application.UI.AchievementPopup
{
    public class ShowAchievementPopupTaskHandler : AsyncOutputRequestHandler<ShowAchievementPopup>
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;

        private readonly Queue<ShowAchievementPopup> _queue = new Queue<ShowAchievementPopup>();
        private readonly Subject<Unit> _completed = new Subject<Unit>();

        protected override async UniTask Handle(ShowAchievementPopup task, CancellationToken ct)
        {
            _queue.Enqueue(task);
            await WaitInQueue(task);

            _title.text = task.Title;
            _description.text = task.Description;

            await _root.DOAnchorPosY(-25, 0.5f).WithCancellation(ct);
            await UniTask.Delay(1000, cancellationToken: ct);
            await _root.DOAnchorPosY(175, 0.5f).WithCancellation(ct);
            _queue.Dequeue();
            _completed.OnNext(Unit.Default);
        }

        private UniTask WaitInQueue(ShowAchievementPopup task)
        {
            if (_queue.Count == 1)
            {
                return UniTask.CompletedTask;
            }

            var tcs = new UniTaskCompletionSource();
            _completed.Where(_ => _queue.Peek() == task)
                .First()
                .Subscribe(_ => tcs.TrySetResult());
            return tcs.Task;
        }
    }
}