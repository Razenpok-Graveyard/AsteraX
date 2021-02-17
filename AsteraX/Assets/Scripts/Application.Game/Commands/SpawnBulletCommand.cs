using UnityEngine;

namespace AsteraX.Application.Game.Commands
{
    public class SpawnBulletCommand
    {
        public SpawnBulletCommand(Vector3 worldPosition, Quaternion direction)
        {
            WorldPosition = worldPosition;
            Direction = direction;
        }
        
        public Vector3 WorldPosition { get; }
        public Quaternion Direction { get; }
    }
}