using AsteraX.Domain.Game;
using Razensoft.Mapper;
using Razensoft.Mediator;

namespace AsteraX.Application.UI.Requests
{
    public class ShowGameOverScreen : IAsyncRequest
    {
        public long Level { get; set; }
        public int Score { get; set; }
        
        public class Mapper : IMapper<GameSession, ShowGameOverScreen>
        {
            public static Mapper Instance { get; } = new Mapper();

            public void Map(GameSession source, ShowGameOverScreen destination)
            {
                destination.Level = source.Level.Id;
                destination.Score = source.Score;
            }
        }
    }
}