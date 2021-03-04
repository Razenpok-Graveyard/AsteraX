using UnityEngine;

namespace AsteraX.Application.Game.Level
{
    public class EmitWhenInBounds : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;

        private LevelBounds _levelBounds;
        private ParticleSystem.EmissionModule _emission;

        private void Start()
        {
            _levelBounds = FindObjectOfType<LevelBounds>();
            _emission = _particleSystem.emission;
        }

        private void LateUpdate()
        {
            _emission.enabled = _levelBounds.Contains(transform.position);
        }
    }
}