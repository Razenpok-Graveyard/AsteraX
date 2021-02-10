using MediatR;
using UnityEngine;

namespace AsteraX.GameSimulation.Commands
{
    public class SpawnAsteroidCommand : IRequest
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