using AsteraX.GameSimulation.Commands;
using UniTaskPubSub;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace AsteraX.GameSimulation.Player
{
    public class PlayerInputController : MonoBehaviour
    {
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            var movement = GetMovement();
            AsyncMessageBus.Default.Publish(new MoveShipInputNotification(movement));

            var mouseScreenPosition = (Vector2) _mainCamera.ScreenToViewportPoint(Input.mousePosition);
            AsyncMessageBus.Default.Publish(new RotateShipCommand(mouseScreenPosition));

            if (CrossPlatformInputManager.GetButtonUp("Fire1"))
            {
                AsyncMessageBus.Default.Publish(new FireCommand(mouseScreenPosition));
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