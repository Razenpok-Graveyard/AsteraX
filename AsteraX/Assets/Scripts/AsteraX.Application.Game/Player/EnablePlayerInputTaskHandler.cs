using AsteraX.Application.Tasks.Game;
using Common.Application.Unity;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class EnablePlayerInputTaskHandler : ApplicationTaskHandler<EnablePlayerInput>
    {
        [SerializeField] private PlayerInputController _playerInputController;
        
        protected override void Handle(EnablePlayerInput task)
        {
            _playerInputController.enabled = true;
        }
    }
}