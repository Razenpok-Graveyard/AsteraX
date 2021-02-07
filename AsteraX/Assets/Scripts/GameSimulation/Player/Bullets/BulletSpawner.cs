using System.Threading;
using AsteraX.GameSimulation.Commands;
using Cysharp.Threading.Tasks;
using MediatR;
using UnityEngine;

namespace AsteraX.GameSimulation.Player.Bullets
{
    public class BulletSpawner : MonoBehaviour, IRequestHandler<SpawnBulletCommand>
    {
        [SerializeField] private BulletSettings _bulletSettings;

        public UniTask<Unit> Handle(SpawnBulletCommand command, CancellationToken cancellationToken)
        {
            var bullet = Instantiate(_bulletSettings.Prefab, command.WorldPosition, command.Direction);
            bullet.Initialize(_bulletSettings.Speed, _bulletSettings.Lifetime);
            return Unit.Task;
        }
    }
}