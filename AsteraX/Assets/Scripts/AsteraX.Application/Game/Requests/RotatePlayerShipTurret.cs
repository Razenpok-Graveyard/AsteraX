using Razensoft.Mediator;
using UnityEngine;

namespace AsteraX.Application.Game.Requests
{
    public class RotatePlayerShipTurret : IRequest
    {
        public Vector2 ScreenPosition { get; set; }
    }
}