using UnityEngine;

namespace AsteraX.Application.Game.Asteroids
{
    [CreateAssetMenu(fileName = "AsteroidSettings", menuName = "AsteraX/Create AsteroidSettings")]
    public class AsteroidSettings : ScriptableObject
    {
        [SerializeField] private GameObject[] _prefabs;
        [SerializeField] private float _speed;
        [SerializeField] private float _rotationSpeed;

        public GameObject[] Prefabs => _prefabs;

        public float Speed => _speed;

        public float RotationSpeed => _rotationSpeed;
    }
}