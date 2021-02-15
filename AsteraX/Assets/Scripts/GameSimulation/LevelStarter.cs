using AsteraX.GameSimulation.Commands;
using UniTaskPubSub;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AsteraX.GameSimulation
{
    public class LevelStarter : MonoBehaviour
    {
        private const int AsteroidCount = 3;

        private void Start()
        {
            for (var i = 0; i < AsteroidCount; i++)
            {
                const int size = 3;
                var position = GetRandomSpawnPosition();
                var direction = Random.rotation;
                AsyncMessageBus.Default.Publish(new SpawnAsteroidCommand(size, position, direction));
            }
        }

        private static Vector3 GetRandomSpawnPosition()
        {
            var minPosition = new Vector2(-15.5f, -8.5f);
            var maxPosition = new Vector2(15.5f, 8.5f);

            Vector2 randomPosition;
            do
            {
                randomPosition = new Vector2(
                    Random.Range(minPosition.x, maxPosition.x),
                    Random.Range(minPosition.y, maxPosition.y)
                );
            } while (IsInsideSafeArea(randomPosition));

            return randomPosition;
        }

        private static bool IsInsideSafeArea(Vector2 position)
        {
            var safeArea = Rect.MinMaxRect(-5, -4, 5, 4);
            return safeArea.Contains(position);
        }
    }
}