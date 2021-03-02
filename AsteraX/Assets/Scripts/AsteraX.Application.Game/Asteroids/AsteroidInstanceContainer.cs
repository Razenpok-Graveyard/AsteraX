using System.Collections.Generic;
using UnityEngine;

namespace AsteraX.Application.Game.Asteroids
{
    public class AsteroidInstanceContainer : MonoBehaviour
    {
        private readonly Dictionary<long, AsteroidInstance> _instances
            = new Dictionary<long, AsteroidInstance>();

        public void Add(long id, AsteroidInstance instance)
        {
            _instances.Add(id, instance);
        }

        public AsteroidInstance Get(long id)
        {
            return _instances[id];
        }

        public void Remove(long id)
        {
            _instances.Remove(id);
        }
    }
}