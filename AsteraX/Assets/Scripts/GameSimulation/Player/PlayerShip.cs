using AsteraX.GameSimulation.Player.Asteroids;
using UnityEngine;

namespace AsteraX.GameSimulation.Player
{
    public class PlayerShip : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Asteroid>() != null)
            {
                Destroy(other.gameObject);
            }
        }
    }
}