using System.Collections.Generic;
using AsteraX.Application.Tasks.Game;
using Common.Application.Unity;
using UnityEngine;

namespace AsteraX.Application.Game.Asteroids
{
    public class AsteroidInstanceContainer : MonoBehaviour
    {
        private readonly Dictionary<long, AsteroidInstance> _instances
            = new Dictionary<long, AsteroidInstance>();

        private void Awake()
        {
            this.Subscribe<DestroyAsteroid>(Handle);
            this.Subscribe<ClearAsteroids>(Handle);
        }

        private void Handle(DestroyAsteroid message)
        {
            var instance = _instances[message.Id];
            var childContainer = instance.GetComponent<AsteroidChildContainer>();
            childContainer.DetachChildren();
            _instances.Remove(message.Id);
            Destroy(instance.gameObject);
        }

        private void Handle(ClearAsteroids task)
        {
            foreach (var instance in _instances.Values)
            {
                Destroy(instance.gameObject);
            }
            
            _instances.Clear();
        }

        public void Add(long id, AsteroidInstance instance)
        {
            _instances.Add(id, instance);
        }
    }
}