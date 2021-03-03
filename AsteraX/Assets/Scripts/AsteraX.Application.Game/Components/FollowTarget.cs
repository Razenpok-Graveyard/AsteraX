using UnityEngine;

namespace AsteraX.Application.Game.Components
{
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        public Transform Transform
        {
            get => _transform;
            set => _transform = value;
        }

        private void Update()
        {
            var currentPosition = transform.position;
            var position = _transform.position;
            position.z = currentPosition.z;
            transform.position = position;
        }
    }
}