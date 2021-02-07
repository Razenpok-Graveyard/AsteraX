﻿using UnityEngine;

namespace AsteraX.GameSimulation.Player
{
    [CreateAssetMenu(menuName = "AsteraX/Create PlayerShipSettings", fileName = "PlayerShipSettings")]
    public class PlayerShipSettings : ScriptableObject
    {
        [SerializeField] private float _maximumSpeed;
        [SerializeField] private float _maximumTilt;

        public float MaximumSpeed => _maximumSpeed;

        public float MaximumTilt => _maximumTilt;
    }
}