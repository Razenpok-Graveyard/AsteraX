using UnityEngine;

namespace AsteraX.Application.Game.Player.Bullets
{
    [CreateAssetMenu(fileName = "BulletSettings", menuName = "AsteraX/Create BulletSettings")]
    public class BulletSettings : ScriptableObject
    {
        [SerializeField] private Bullet _prefab;
        [SerializeField] private float _speed;
        [SerializeField] private float _lifetime;

        public Bullet Prefab => _prefab;

        public float Speed => _speed;

        public float Lifetime => _lifetime;
    }
}