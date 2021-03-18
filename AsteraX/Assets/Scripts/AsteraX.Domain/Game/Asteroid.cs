using System;
using System.Collections.ObjectModel;
using Razensoft.Domain;

namespace AsteraX.Domain.Game
{
    public class Asteroid : Entity
    {
        public Asteroid(long id, int size) : base(id)
        {
            Contract.Requires(id >= 0, "id >= 0");
            Contract.Requires(size >= 1 && size <= 3, "size >= 1 && size <= 3");

            Size = size;
        }

        public int Size { get; }

        public Collection<Asteroid> Children { get; } = new Collection<Asteroid>();

        public int Score
        {
            get
            {
                switch (Size)
                {
                    case 1:
                        return 300;
                    case 2:
                        return 200;
                    case 3:
                        return 100;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}