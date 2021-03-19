using AsteraX.Application.Game.Requests;
using Razensoft.Mediator;
using UnityEngine;

namespace AsteraX.Application.Game.Levels
{
    public class UnpauseGameRequestHandler : OutputRequestHandler<UnpauseGame>
    {
        protected override void Awake()
        {
            base.Awake();
            Time.timeScale = 0;
        }
        
        protected override void Handle(UnpauseGame task)
        {
            Time.timeScale = 1;
        }
    }
}