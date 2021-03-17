using Common.Application;
using UnityEngine;

namespace AsteraX.Application.Game.Requests
{
    public class FirePlayerShipTurret : IRequest
    {
        public Vector2 ScreenPosition { get; set; }
    }
}