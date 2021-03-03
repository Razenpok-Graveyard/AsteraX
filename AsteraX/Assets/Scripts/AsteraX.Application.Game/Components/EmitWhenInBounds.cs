using UnityEngine;

namespace AsteraX.Application.Game.Components
{
    public class EmitWhenInBounds : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        private void LateUpdate()
        {
            var emission = _particleSystem.emission;
            emission.enabled = IsInBounds();
        }

        private bool IsInBounds()
        {
            var bounds = Rect.MinMaxRect(-16, -9, 16, 9);
            Vector2 position = transform.position;
            return bounds.Contains(position);
        }
    }
}