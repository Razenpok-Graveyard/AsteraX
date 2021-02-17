using UnityEngine;

namespace AsteraX.Application.Game.Commands
{
    public class TranslateGameFieldObjectCommand
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