﻿using AsteraX.Application.UI.Requests;
using Razensoft.Mediator;

namespace AsteraX.Application.UI.MainMenu
{
    public class CloseMainMenuScreenRequestHandler : OutputRequestHandler<HideMainMenuScreen>
    {
        protected override void Handle(HideMainMenuScreen task)
        {
            gameObject.SetActive(false);
        }
    }
}