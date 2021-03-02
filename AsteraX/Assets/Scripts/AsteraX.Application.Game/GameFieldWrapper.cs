using UnityEngine;

namespace AsteraX.Application.Game
{
    public class GameFieldWrapper : MonoBehaviour
    {
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
    }
}