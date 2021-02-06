using MediatR;
using UnityEngine;

namespace AsteraX.GameSimulation.Commands
{
    public class MoveShipCommand : IRequest
    {
        public MoveShipCommand(Vector2 movement)
        {
            Movement = movement;
        }

        public Vector2 Movement { get; }
    }
}