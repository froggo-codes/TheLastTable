using System;

namespace LastTable.Core
{
    /// <summary>
    /// One guest at the table: their years, their card, and whether they're still in.
    /// </summary>
    public sealed class Player
    {
        public const int TotalYears = 100;

        public string Name { get; }
        public int Seat { get; }
        public int Years { get; private set; }
        public Card Hand { get; private set; }
        public bool HasFolded { get; private set; }
        public bool IsGhost { get; private set; }

        public bool HasCard => Hand != null;
        
        // Still in the hand: alive, holding a card, not folded
        public bool IsInRound => !IsGhost && HasCard && !HasFolded;

        public Player(string name, int seat, int startingYears)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("A player needs a name.", nameof(name));
            }

            if (startingYears < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(startingYears), "A player starts with at least 1 year.");
            }

            Name = name;
            Seat = seat;
            Years = startingYears;
        }
        
        // Rulebook setup: 100 years split evenly (33,25,20 each - depending on # of Players)
        public static int StartingYearsFor(int playerCount)
        {
            if (playerCount < 3 || playerCount > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(playerCount), "The Last Table seats 3 to 5 players.");
            }

            return TotalYears / playerCount;
        }

        /// <summary>
        /// Pays up to the amount asked and returns what was actually paid.
        /// A player can't got below 0, so a short player pays everything they have
        /// </summary>
        public int Pay(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Can't pay a negative amount.");
            }

            int paid = Math.Min(amount, Years);
            Years -= paid;
            return paid;
        }

        public void Receive(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Can't receive a negative amount.");
            }

            Years += amount;
        }

        public void TakeCard(Card card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            if (IsGhost)
            {
                throw new InvalidOperationException($"{Name} is a ghost and can't be dealt a card.");
            }

            Hand = card;
        }
        
        // A folded card stays secret: the hand is kept, but the player is out of the round.
        public void Fold()
        {
            HasFolded = true;
        }
        
        // Called after the payout: clears the hand, and anyone at 0 Years becomes a Ghost
        public void EndRound()
        {
            Hand = null;
            HasFolded = false;

            if (Years == 0)
            {
                IsGhost = true;
            }
        }
        
        // Prints as "Ana (25 Years)" or "Cal (Ghost)"
        public override string ToString()
        {
            return IsGhost ? $"{Name} (Ghost)" : $"{Name} ({Years} Years)";
        }
    }
}