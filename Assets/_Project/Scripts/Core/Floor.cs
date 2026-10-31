using System;
using System.Collections.Generic;

namespace LastTable.Core
{
    /// <summary>
    /// One floor of the House: its die, its ante, and its betting limit.
    /// </summary>
    public sealed class Floor
    {
        public int Number { get; }
        public string Name { get; }
        public int DieSides { get; }
        public int Ante { get; }
        public int MaxRaise { get; }
        
        // Each player rolls the floor's die twice
        public int MinRoll => 2;
        public int MaxRoll => DieSides * 2;

        public Floor(int number, string name, int dieSides, int ante, int maxRaise)
        {
            if (dieSides < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(dieSides), "A die needs at least 2 sides.");
            }

            Number = number;
            Name = name;
            DieSides = dieSides;
            Ante = ante;
            MaxRaise = maxRaise;
        }
        
        // Prints as "Floor 2: The Bar (2d6, ante 2, max raise 2)"
        public override string ToString()
        {
            return $"Floor {Number}: {Name} (2d{DieSides}, ante {Ante}, max raise {MaxRaise})";
        }
    }

    /// <summary>
    /// The Climb: all six floors, Lobby to Penthouse, straight from the rulebook.
    /// </summary>
    public static class Climb
    {
        public const int RoundsPerFloor = 2;
        
        // Read-only from outside: other scripts can look floors up but never add or remove one
        public static readonly IReadOnlyList<Floor> Floors = new List<Floor>
        {
            new Floor(1, "The Lobby", 4, 1, 1),
            new Floor(2, "The Bar", 6, 2, 2),
            new Floor(3, "The Parlor", 8, 3, 3),
            new Floor(4, "The Vault", 10, 4, 4),
            new Floor(5, "The Pit", 12, 5, 5),
            new Floor(6, "The Penthouse", 20, 6, 6)
        };

        public static Floor First => Floors[0];
        public static Floor Top => Floors[Floors.Count - 1];
        
        // Floors are numbered from 1, but lists count from 0
        public static Floor Get(int number)
        {
            if (number < 1 || number > Floors.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(number), $"There is no floor {number}.");
            }

            return Floors[number - 1];
        }

        public static bool IsTop(Floor floor) => floor.Number == Top.Number;
        
        // The floor above, or null from the Penthouse (the next stop is the Exit)
        public static Floor Above(Floor floor) => IsTop(floor) ? null : Get(floor.Number + 1);
    }
}
