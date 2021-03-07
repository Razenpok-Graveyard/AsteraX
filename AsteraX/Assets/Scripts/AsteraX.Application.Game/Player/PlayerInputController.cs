using Common.Application;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    public class PlayerInputController : MonoBehaviour
    {
        private Camera _mainCamera;
        private readonly MovePlayerShip _movePlayerShipTask
            = new MovePlayerShip();
        private readonly RotatePlayerShipTurret _rotatePlayerShipTurretTask
            = new RotatePlayerShipTurret();
        private readonly FirePlayerShipTurret _firePlayerShipTurretTask
            = new FirePlayerShipTurret();

        private IApplicationTaskPublisher _taskPublisher;

        [Inject]
        public void Construct(IApplicationTaskPublisher applicationTaskPublisher)
        {
            _taskPublisher = applicationTaskPublisher;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            var movement = GetMovement();
            _movePlayerShipTask.Movement = movement;
            _taskPublisher.PublishTask(_movePlayerShipTask);

            var mouseScreenPosition = (Vector2) _mainCamera.ScreenToViewportPoint(Input.mousePosition);
            _rotatePlayerShipTurretTask.ScreenPosition = mouseScreenPosition;
            _taskPublisher.PublishTask(_rotatePlayerShipTurretTask);

            if (CrossPlatformInputManager.GetButtonUp("Fire1"))
            {
                _firePlayerShipTurretTask.ScreenPosition = mouseScreenPosition;
                _taskPublisher.PublishTask(_firePlayerShipTurretTask);
            }
        }

        private static Vector2 GetMovement()
        {
            var horizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");
            var verticalInput = CrossPlatformInputManager.GetAxis("Vertical");
            
            var inputVector = new Vector2(horizontalInput, verticalInput);
            var direction = inputVector.normalized;
            var speed = Mathf.Max(Mathf.Abs(horizontalInput), Mathf.Abs(verticalInput));
            return direction * speed;
        }
    }
}