﻿using AsteraX.Application.UI.Requests;
using Razensoft.Mediator;

namespace AsteraX.Application.UI.MainMenu
{
    public class ShowMainMenuScreenRequestHandler : OutputRequestHandler<ShowMainMenuScreen>
    {
        protected override void Handle(ShowMainMenuScreen task)
        {
            gameObject.SetActive(true);
        }
    }
}