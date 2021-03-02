using AsteraX.Application.Common;
using UnityEngine;

namespace AsteraX.Application.Game
{
    public class SpawnBullet : IApplicationTask
    {
        public Vector3 WorldPosition { get; set; }
        public Vector3 Direction { get; set; }
    }
}