using UnityEngine;

namespace AsteraX.GameSimulation.Asteroids
{
    [CreateAssetMenu(fileName = "AsteroidSettings", menuName = "AsteraX/Create AsteroidSettings")]
    public class AsteroidSettings : ScriptableObject
    {
        [SerializeField] private Asteroid[] _prefabs;
        [SerializeField] private float _speed;
        [SerializeField] private float _rotationSpeed;

        public Asteroid[] Prefabs => _prefabs;

        public float Speed => _speed;

        public float RotationSpeed => _rotationSpeed;
    }
}