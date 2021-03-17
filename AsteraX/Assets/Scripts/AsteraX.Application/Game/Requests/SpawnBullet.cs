using Common.Application;
using UnityEngine;

namespace AsteraX.Application.Game.Requests
{
    public class SpawnBullet : IRequest
    {
        public Vector3 WorldPosition { get; set; }
        public Vector3 Direction { get; set; }
    }
}