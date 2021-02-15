﻿using UnityEngine;

namespace AsteraX.GameSimulation.Commands
{
    public class MoveShipInputNotification
    {
        public MoveShipInputNotification(Vector2 movement)
        {
            Movement = movement;
        }

        public Vector2 Movement { get; }
    }
}