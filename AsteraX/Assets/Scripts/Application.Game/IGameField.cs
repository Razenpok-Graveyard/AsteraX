using AsteraX.Application.Game.Asteroids;

namespace AsteraX.Application.Game
{
    public interface IGameField
    {
        void Destroy(Asteroid asteroid);
        void DestroyPlayerShip();
    }
}