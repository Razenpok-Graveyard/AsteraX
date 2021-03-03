using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class DestroyPlayerShipTaskHandler : ApplicationTaskHandler<DestroyPlayerShip>
    {
        [SerializeField] private GameObject _playerShip;

        protected override void Handle(DestroyPlayerShip message)
        {
            _playerShip.SetActive(false);
        }
    }
}