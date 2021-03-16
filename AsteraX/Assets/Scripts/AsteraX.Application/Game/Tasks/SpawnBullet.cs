using Common.Application;
using UnityEngine;

namespace AsteraX.Application.Game.Tasks
{
    public class SpawnBullet : IApplicationTask
    {
        public Vector3 WorldPosition { get; set; }
        public Vector3 Direction { get; set; }
    }
}