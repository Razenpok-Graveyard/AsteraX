using System;
using System.Collections.ObjectModel;
using CSharpFunctionalExtensions;

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
                return Size switch
                {
                    1 => 300,
                    2 => 200,
                    3 => 100,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}