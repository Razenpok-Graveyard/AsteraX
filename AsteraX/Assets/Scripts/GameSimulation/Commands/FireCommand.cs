using UnityEngine;

namespace AsteraX.GameSimulation.Commands
{
    public class FireCommand
    {
        public FireCommand(Vector2 screenPosition)
        {
            ScreenPosition = screenPosition;
        }
        
        public Vector2 ScreenPosition { get; }
    }
}