using System.Collections.Generic;
using AsteraX.Application.Common;
using UnityEngine;

namespace AsteraX.Application.Game
{
    public class SpawnAsteroid : IApplicationTask
    {
        public long Id { get; set; }
        public int Size { get; set; }
        public Vector3 WorldPosition { get; set; }
        public Quaternion Direction { get; set; }
        public List<ChildAsteroid> Children { get; set; }

        public class ChildAsteroid
        {
            public long Id { get; set; }
            public int Size { get; set; }
        }
    }
}