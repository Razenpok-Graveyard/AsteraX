using UnityEngine;

namespace AsteraX.Application.Game.Asteroids
{
    public class DestroyAsteroidTaskHandler : ApplicationTaskHandler<DestroyAsteroid>
    {
        [SerializeField] private AsteroidInstanceContainer _instanceContainer;

        protected override void Handle(DestroyAsteroid message)
        {
            var instance = _instanceContainer.Get(message.Id);
            var childContainer = instance.GetComponent<AsteroidChildContainer>();
            childContainer.DetachChildren();
            _instanceContainer.Remove(message.Id);
            Destroy(instance.gameObject);
        }
    }
}