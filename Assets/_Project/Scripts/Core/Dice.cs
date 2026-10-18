using System;
using Unity.VisualScripting;

namespace LastTable.Core
{
    /// <summary>
    /// Rolls dice. Static: it holds no data of its own, so there's nothing to create.
    /// </summary>
    public static class Dice
    {
        public const int Percentile = 100;

        /// <summary>Rolls one die andd returns a value from 1 to siddes.</summary>
        public static int Roll(Random rng, int sides)
        {
            if (rng == null)
            {
                throw new ArgumentNullException(nameof(rng));
            }

            if (sides < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(sides), "A die needs at least 2 sides.");
            }

            // Next(min, max) includes min but excludes max, hence sides + 1
            return rng.Next(1, sides + 1);
        }
        
        // The Exit roll: two d10s read as 01-100 ("00" + "0" = 100), same odds as one d100
        public static int RollPecentile(Random rng) => Roll(rng, Percentile);
    }

    public enum DieSlot
    {
       First,
       Second
    }

    /// <summary>
    /// The two dice a player rolls each round. Kept separate (not just a total)
    /// because the Jack swaps single dice andd the Queen rerolls single dice.
    /// </summary>
    public sealed class DicePair
    {
        private readonly Random rng;

        public int Sides { get; }
        public int First { get; private set; }
        public int Second { get; private set; }

        public int Total => First + Second;
        public DieSlot LowestSlot => First <= Second ? DieSlot.First : DieSlot.Second;
        public DieSlot HighestSlot => First > Second ? DieSlot.First : DieSlot.Second;

        public DicePair(int sides, Random rng)
        {
            if (sides < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(sides), " A die needs at least 2 sides.");
            }

            Sides = sides;
            this.rng = rng ?? new Random();
        }

        public void RollBoth()
        {
            Reroll(DieSlot.First);
            Reroll(DieSlot.Second);
        }

        public void Reroll(DieSlot slot)
        {
            Set(slot, Dice.Roll(rng, Sides));
        }

        public int Get(DieSlot slot)
        {
            return slot == DieSlot.First ? First : Second;
        }
        
        //Used by swaps (Jack) as well as rolls, so it guards the value range
        public void Set(DieSlot slot, int value)
        {
            if (value < 1 || value > Sides)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"{value} can't appear on a d{Sides}.");
            }

            if (slot == DieSlot.First)
            {
                First = value;
            }
            else
            {
                Second = value;
            }
        }
        
        // Print as "4+2=6"
        public override string ToString()
        {
            return $"{First} + {Second} = {Total}";
        }
    }
}