using System;

namespace LastTable.Core
{
    /// <summary>
    /// Suit Values double as tie-break strength: Spades > Hearts > Diamonds > Clubs.
    /// Jokers have no suit and lose every tie.
    /// </summary>
    public enum Suit
    {
        None = 0,
        Clubs = 1,
        Diamonds = 2,
        Hearts = 3,
        Spades = 4
    }

    // Numbered from 2 so (int)Rank gives a number card's face value
    public enum Rank
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace,
        Joker
    }

    public sealed class Card
    {
        public Rank Rank { get; }
        public Suit Suit { get; }

        public Card(Rank rank, Suit suit)
        {
            // A Joker must have no suit, and every other card must have one.
            bool isJoker = rank == Rank.Joker;
            bool hasNoSuit = suit == Suit.None;

            if (isJoker != hasNoSuit)
            {
                throw new ArgumentException($"Invalid Card: {rank} of {suit}");
            }

            Rank = rank;
            Suit = suit;
        }

        public bool IsJoker => Rank == Rank.Joker;

        // 10, J, Q, K, A, and Joker all have powers
        public bool HasPower => Rank >= Rank.Ten;

        // A Joker's value is named by its player at showdown, so its base is 0
        public int BaseValue => Rank switch
        {
            Rank.Ace => 11,
            Rank.Ten or Rank.Jack or Rank.Queen or Rank.King => 10,
            Rank.Joker => 0,
            _ => (int)Rank
        };

        public string Title => Rank switch
        {
            Rank.Ten => "The Witness",
            Rank.Ace => "The Soul",
            Rank.Jack => "The Trickster",
            Rank.Queen => "The Widow",
            Rank.King => "The Collector",
            Rank.Joker => "The Stranger",
            _ => "The Crowd"
        };

        // Higher wins ties; Jokers are 0
        public int SuitStrength => (int)Suit;

        // Prints as "Q♣", "10♠" or "Joker"
        public override string ToString()
        {
            if (IsJoker) return "Joker";

            string rankText = Rank switch
            {
                Rank.Ace => "A",
                Rank.King => "K",
                Rank.Queen => "Q",
                Rank.Jack => "J",
                _ => ((int)Rank).ToString()
            };

            string suitText = Suit switch
            {
                Suit.Spades => "♠",
                Suit.Hearts => "♥",
                Suit.Diamonds => "♦",
                Suit.Clubs => "♣",
                _ => "?"
            };

            return rankText + suitText;
        }
    }
}
