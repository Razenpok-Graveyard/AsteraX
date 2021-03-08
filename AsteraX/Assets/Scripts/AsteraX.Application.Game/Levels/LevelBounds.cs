using System.Linq;
using AsteraX.Application.Game.Asteroids;
using UnityEngine;

namespace AsteraX.Application.Game.Levels
{
    public class LevelBounds : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _transform;
        [SerializeField] private Vector2 _safeAreaNormalizedSize;

        private Rect _screenRect;
        
        private void Start()
        {
            UpdateScreenRect();
        }

        private void Update()
        {
#if UNITY_EDITOR
            UpdateScreenRect();
#endif
        }

        private void UpdateScreenRect()
        {
            var topRightCorner = new Vector2(1, 1);
            var max = _camera.ViewportToWorldPoint(topRightCorner);
            var topRightCorner2 = new Vector2(0, 0);
            var min = _camera.ViewportToWorldPoint(topRightCorner2);
            _screenRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            _transform.localScale = new Vector3(
                _screenRect.width,
                _screenRect.height,
                _transform.localScale.z
            );
        }

        private void OnTriggerExit(Collider other)
        {
            var inverseTransformPoint = transform.InverseTransformPoint(other.transform.position);
            if (Mathf.Abs(inverseTransformPoint.x) > 0.5f)
            {
                inverseTransformPoint.x *= -1;
            }
            if (Mathf.Abs(inverseTransformPoint.y) > 0.5f)
            {
                inverseTransformPoint.y *= -1;
            }

            other.transform.position = transform.TransformPoint(inverseTransformPoint);
        }

        public Vector2 GetRandomPositionOutsideSafeArea(Vector2 padding)
        {
            Vector2 randomPosition;
            do
            {
                randomPosition = GetRandomPosition(padding);
            } while (IsInsideSafeArea(randomPosition));

            return randomPosition;
        }

        public Vector2 FindSafePosition(Vector2 padding)
        {
            var asteroids = FindObjectsOfType<AsteroidInstance>();
            for (int i = 0; i < 100; i++)
            {
                var point = GetRandomPosition(padding);
                var distances = asteroids.Select(a => Vector3.Distance(a.transform.position, point));
                if (distances.All(d => d > 8))
                {
                    return point;
                }
            }
            
            return GetRandomPosition(padding);
        }

        private Vector2 GetRandomPosition(Vector2 padding)
        {
            var paddedRect = GetPaddedRect(padding);
            return new Vector2(
                Random.Range(paddedRect.xMin, paddedRect.xMax),
                Random.Range(paddedRect.yMin, paddedRect.yMax)
            );
        }

        public bool Contains(Vector2 position)
        {
            return _screenRect.Contains(position);
        }

        private bool IsInsideSafeArea(Vector2 position)
        {
            var padding = (_screenRect.size - _safeAreaNormalizedSize) / 2;
            var paddedRect = GetPaddedRect(padding);
            return paddedRect.Contains(position);
        }

        private Rect GetPaddedRect(Vector2 padding)
        {
            return Rect.MinMaxRect(
                _screenRect.xMin + padding.x,
                _screenRect.yMin + padding.y,
                _screenRect.xMax - padding.x,
                _screenRect.yMax - padding.y
            );
        }
    }
}