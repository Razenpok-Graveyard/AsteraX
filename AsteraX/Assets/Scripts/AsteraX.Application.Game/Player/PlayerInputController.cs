using AsteraX.Application.Game.Requests;
using Razensoft.Mediator;
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

        private IOutputMediator _mediator;

        [Inject]
        public void Construct(IOutputMediator mediator)
        {
            _mediator = mediator;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            enabled = false;
            this.RegisterHandler<DisablePlayerInput>(_ =>
            {
                _movePlayerShipTask.Movement = Vector2.zero;
                _mediator.Send(_movePlayerShipTask);
                enabled = false;
            });
            this.RegisterHandler<EnablePlayerInput>(_ => enabled = true);
        }

        private void Update()
        {
            var movement = GetMovement();
            _movePlayerShipTask.Movement = movement;
            _mediator.Send(_movePlayerShipTask);

            var mouseScreenPosition = (Vector2) _mainCamera.ScreenToViewportPoint(Input.mousePosition);
            _rotatePlayerShipTurretTask.ScreenPosition = mouseScreenPosition;
            _mediator.Send(_rotatePlayerShipTurretTask);

            if (CrossPlatformInputManager.GetButtonUp("Fire1"))
            {
                _firePlayerShipTurretTask.ScreenPosition = mouseScreenPosition;
                _mediator.Send(_firePlayerShipTurretTask);
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