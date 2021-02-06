using System.Threading;
using System.Threading.Tasks;
using AsteraX.GameSimulation.Commands;
using MediatR;
using UnityEngine;

namespace AsteraX.GameSimulation.Player
{
    public class TurretRotationPresenter : MonoBehaviour, IRequestHandler<RotateShipCommand>
    {
        [SerializeField] private Transform _turret;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public Task<Unit> Handle(RotateShipCommand command, CancellationToken cancellationToken)
        {
            if (!isActiveAndEnabled)
            {
                return Unit.Task;
            }

            var positionOnScreen = _mainCamera.WorldToViewportPoint(_turret.position);
            var angle = AngleBetweenPoints(positionOnScreen, command.ScreenPosition);
            var rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            // Offset because of model
            _turret.rotation = rotation * Quaternion.Euler(0, 0, 90);
            return Unit.Task;
        }

        private static float AngleBetweenPoints(Vector2 a, Vector2 b)
        {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }
    }
}