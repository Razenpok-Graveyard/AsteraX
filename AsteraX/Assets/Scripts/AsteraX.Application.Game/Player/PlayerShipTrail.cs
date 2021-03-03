using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class PlayerShipTrail : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        public void UpdateDirection(Vector2 movement)
        {
            var angle = AngleBetweenPoints(Vector2.zero, movement);
            _particleSystem.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        private static float AngleBetweenPoints(Vector2 a, Vector2 b)
        {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }
    }
}