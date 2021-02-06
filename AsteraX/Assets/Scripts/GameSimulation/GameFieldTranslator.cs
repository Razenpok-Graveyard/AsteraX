using System.Threading;
using System.Threading.Tasks;
using AsteraX.GameSimulation.Commands;
using MediatR;
using UnityEngine;

namespace AsteraX.GameSimulation
{
    public class GameFieldTranslator : MonoBehaviour, IRequestHandler<TranslateGameFieldObjectCommand>
    {
        public Task<Unit> Handle(TranslateGameFieldObjectCommand command, CancellationToken cancellationToken)
        {
            var go = command.GameObject;
            go.transform.Translate(command.Translation);
            // Welp
            var position = go.transform.position;
            position.z = 0;
            go.transform.position = position;
            return Unit.Task;
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