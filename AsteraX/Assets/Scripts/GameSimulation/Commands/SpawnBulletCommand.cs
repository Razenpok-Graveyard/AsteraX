using UnityEngine;

namespace AsteraX.GameSimulation.Commands
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