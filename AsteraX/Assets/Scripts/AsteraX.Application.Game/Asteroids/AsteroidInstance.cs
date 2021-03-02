using UnityEngine;

namespace AsteraX.Application.Game.Asteroids
{
    public class AsteroidInstance : MonoBehaviour
    {
        public long Id { get; private set; }

        public static AsteroidInstance AddTo(GameObject gameObject, long id)
        {
            var instance = gameObject.AddComponent<AsteroidInstance>();
            instance.Id = id;
            return instance;
        }
    }
}