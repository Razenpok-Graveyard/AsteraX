using MediatR;
using UnityEngine;

namespace AsteraX.GameSimulation.Commands
{
    public class RotateShipCommand : IRequest
    {
        public RotateShipCommand(Vector2 screenPosition)
        {
            ScreenPosition = screenPosition;
        }
        
        public Vector2 ScreenPosition { get; }
    }
}