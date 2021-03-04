using UnityEngine;

namespace AsteraX.Application.Game.Asteroids
{
    [CreateAssetMenu(fileName = "AsteroidSettings", menuName = "AsteraX/Create AsteroidSettings")]
    public class AsteroidSettings : ScriptableObject
    {
        [SerializeField] private GameObject[] _prefabs;
        [SerializeField] private float _minSpeed;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private float _minRotationSpeed;
        [SerializeField] private float _maxRotationSpeed;

        public GameObject[] Prefabs => _prefabs;

        public float MinSpeed => _minSpeed;

        public float MaxSpeed => _maxSpeed;

        public float MinRotationSpeed => _minRotationSpeed;

        public float MaxRotationSpeed => _maxRotationSpeed;
    }
}