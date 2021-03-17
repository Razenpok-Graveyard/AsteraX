using AsteraX.Application.Game.Requests;
using Common.Application.Unity;
using UnityEngine;

namespace AsteraX.Application.Game.Levels
{
    public class UnpauseGameTaskHandler : OutputRequestHandler<UnpauseGame>
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