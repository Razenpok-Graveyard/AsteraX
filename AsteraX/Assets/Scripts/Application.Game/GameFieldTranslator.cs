using AsteraX.Application.Game.Commands;
using UnityEngine;

namespace AsteraX.Application.Game
{
    public class GameFieldTranslator : MonoBehaviour
    {
        private void Awake()
        {
            this.Subscribe<TranslateGameFieldObjectCommand>(Handle);
        }

        private void Handle(TranslateGameFieldObjectCommand command)
        {
            var go = command.GameObject;
            go.transform.Translate(command.Translation);
            // Welp
            var position = go.transform.position;
            position.z = 0;
            go.transform.position = position;
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
    }
}