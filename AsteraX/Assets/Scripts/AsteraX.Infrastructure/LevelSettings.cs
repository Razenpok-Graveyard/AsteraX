using System;
using System.Collections.Generic;
using UnityEngine;

namespace AsteraX.Infrastructure
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "AsteraX/Create LevelSettings")]
    public class LevelSettings : ScriptableObject
    {
        [SerializeField] private List<SingleLevelSettings> _levels;

        public List<SingleLevelSettings> Levels => _levels;

        [Serializable]
        public class SingleLevelSettings
        {
            [SerializeField] private int _asteroidCount;
            [SerializeField] private int _asteroidChildCount;

            public int AsteroidCount => _asteroidCount;

            public int AsteroidChildCount => _asteroidChildCount;
        }
    }
}