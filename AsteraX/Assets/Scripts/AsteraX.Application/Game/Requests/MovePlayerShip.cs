using Razensoft.Mediator;
using UnityEngine;

namespace AsteraX.Application.Game.Requests
{
    public class MovePlayerShip : IRequest
    {
        public Vector2 Movement { get; set; }
    }
}