using AsteraX.Application.Game.Requests;
using Razensoft.Mediator;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class DestroyPlayerShipRequestHandler : OutputRequestHandler<DestroyPlayerShip>
    {
        [SerializeField] private GameObject _playerShip;
        [SerializeField] private ParticleSystem _exhaustTrail;
        [SerializeField] private ParticleSystem _disappearEffect;

        protected override void Handle(DestroyPlayerShip message)
        {
            _playerShip.SetActive(false);
            var emission = _exhaustTrail.emission;
            emission.enabled = false;
            var position = _playerShip.transform.position;
            position.z = _disappearEffect.transform.position.z;
            var effect = Instantiate(_disappearEffect, position, Quaternion.identity);
            DestroyWhenDone(effect).Forget();
        }

        private static async UniTask DestroyWhenDone(ParticleSystem particleSystem)
        {
            await UniTask.WaitWhile(particleSystem.IsAlive);
            Destroy(particleSystem);
        }
    }
}