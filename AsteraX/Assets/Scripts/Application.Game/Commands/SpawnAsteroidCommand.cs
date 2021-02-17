using UnityEngine;

namespace AsteraX.Application.Game.Commands
{
    public class SpawnAsteroidCommand
    {
        public SpawnAsteroidCommand(int size, Vector3 worldPosition, Quaternion direction)
        {
            Size = size;
            WorldPosition = worldPosition;
            Direction = direction;
        }
        
        public int Size { get; }
        public Vector3 WorldPosition { get; }
        public Quaternion Direction { get; }
    }
}