using Common.Application;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using VContainer;

namespace AsteraX.Application.Game.Player
{
    public class PlayerInputController : MonoBehaviour
    {
        private Camera _mainCamera;
        private readonly MovePlayerShip _movePlayerShipMessage
            = new MovePlayerShip();
        private readonly RotatePlayerShipTurret _rotatePlayerShipTurretMessage
            = new RotatePlayerShipTurret();
        private readonly FirePlayerShipTurret _firePlayerShipTurret
            = new FirePlayerShipTurret();

        private IApplicationTaskPublisher _applicationTaskPublisher;

        [Inject]
        public void Construct(IApplicationTaskPublisher applicationTaskPublisher)
        {
            _applicationTaskPublisher = applicationTaskPublisher;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            var movement = GetMovement();
            _movePlayerShipMessage.Movement = movement;
            _applicationTaskPublisher.Publish(_movePlayerShipMessage);

            var mouseScreenPosition = (Vector2) _mainCamera.ScreenToViewportPoint(Input.mousePosition);
            _rotatePlayerShipTurretMessage.ScreenPosition = mouseScreenPosition;
            _applicationTaskPublisher.Publish(_rotatePlayerShipTurretMessage);

            if (CrossPlatformInputManager.GetButtonUp("Fire1"))
            {
                _firePlayerShipTurret.ScreenPosition = mouseScreenPosition;
                _applicationTaskPublisher.Publish(_firePlayerShipTurret);
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