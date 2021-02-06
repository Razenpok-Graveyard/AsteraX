using MediatR;
using UnityEngine;

namespace AsteraX.GameSimulation.Commands
{
    public class FireCommand : IRequest
    {
        public FireCommand(Vector2 screenPosition)
        {
            ScreenPosition = screenPosition;
        }
        
        public Vector2 ScreenPosition { get; }
    }
}