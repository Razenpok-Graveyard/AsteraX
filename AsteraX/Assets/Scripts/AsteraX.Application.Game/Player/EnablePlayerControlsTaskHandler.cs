using AsteraX.Application.Tasks.Game;
using Common.Application.Unity;
using UnityEngine;

namespace AsteraX.Application.Game.Player
{
    public class EnablePlayerControlsTaskHandler : ApplicationTaskHandler<EnablePlayerControls>
    {
        [SerializeField] private PlayerInputController _playerInputController;
        
        protected override void Handle(EnablePlayerControls task)
        {
            _playerInputController.enabled = true;
        }
    }
}