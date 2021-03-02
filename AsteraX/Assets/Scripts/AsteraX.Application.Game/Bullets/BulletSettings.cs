using UnityEngine;

namespace AsteraX.Application.Game.Bullets
{
    [CreateAssetMenu(fileName = "BulletSettings", menuName = "AsteraX/Create BulletSettings")]
    public class BulletSettings : ScriptableObject
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private float _speed;
        [SerializeField] private float _lifetime;

        public GameObject Prefab => _prefab;

        public float Speed => _speed;

        public float Lifetime => _lifetime;
    }
}