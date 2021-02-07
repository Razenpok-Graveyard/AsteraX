﻿using AsteraX.GameSimulation.Commands;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using static MediatR.MediatorSingleton;

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
            Send(new MoveShipCommand(movement));

            var mouseScreenPosition = (Vector2) _mainCamera.ScreenToViewportPoint(Input.mousePosition);
            Send(new RotateShipCommand(mouseScreenPosition));

            if (CrossPlatformInputManager.GetButtonUp("Fire1"))
            {
                Send(new FireCommand(mouseScreenPosition));
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