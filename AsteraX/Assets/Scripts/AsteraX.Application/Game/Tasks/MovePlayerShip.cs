using Common.Application;
using UnityEngine;

namespace AsteraX.Application.Game.Tasks
{
    public class MovePlayerShip : IApplicationTask
    {
        public Vector2 Movement { get; set; }
    }
}