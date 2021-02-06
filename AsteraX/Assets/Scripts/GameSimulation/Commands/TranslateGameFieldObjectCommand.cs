using MediatR;
using UnityEngine;

namespace AsteraX.GameSimulation.Commands
{
    public class TranslateGameFieldObjectCommand : IRequest
    {
        public TranslateGameFieldObjectCommand(GameObject gameObject, Vector3 translation)
        {
            GameObject = gameObject;
            Translation = translation;
        }

        public GameObject GameObject { get; }

        public Vector3 Translation { get; }
    }
}