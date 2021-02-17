using UnityEngine;

namespace AsteraX.Application.Game.Commands
{
    public class RotateShipCommand
    {
        public RotateShipCommand(Vector2 screenPosition)
        {
            ScreenPosition = screenPosition;
        }
        
        public Vector2 ScreenPosition { get; }
    }
}