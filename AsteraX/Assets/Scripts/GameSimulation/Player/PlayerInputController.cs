using AsteraX.GameSimulation.Commands;
using AsteraX.Mediator.Assets.Scripts;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using VContainer;

namespace AsteraX.GameSimulation.Player
{
    public class PlayerInputController : MonoBehaviour
    {
        private ISender _sender;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        [Inject]
        public void Construct(ISender sender) => _sender = sender;

        private void Update()
        {
            var movement = GetMovement();
            _sender.Send(new MoveShipCommand(movement));

            var mouseScreenPosition = (Vector2) _mainCamera.ScreenToViewportPoint(Input.mousePosition);
            _sender.Send(new RotateShipCommand(mouseScreenPosition));

            if (CrossPlatformInputManager.GetButtonUp("Fire1"))
            {
                _sender.Send(new FireCommand(mouseScreenPosition));
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