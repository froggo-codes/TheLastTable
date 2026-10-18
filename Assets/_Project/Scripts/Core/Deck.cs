using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.U2D.Physics;
using Unity.VisualScripting;

namespace LastTable.Core
{
    /// <summary>
    /// The 54-card deck (52 + 2 Jokers). Rebuilt and reshuffled every round.
    /// </summary>
    public sealed class Deck
    {
        public const int FullSize = 54;

        private readonly List<Card> cards = new List<Card>();
        private readonly Random rng;

        public int Count => cards.Count;

        // RNG is passed in so tests can use a fixed seed and replay the same shuffle
        public Deck(Random rng)
        {
            this.rng = rng ?? new Random();
            Reset();
        }

        public void Reset()
        {
            cards.Clear();

            for (Suit suit = Suit.Clubs; suit <= Suit.Spades; suit++)
            {
                for (Rank rank = Rank.Two; rank <= Rank.Ace; rank++)
                {
                    cards.Add(new Card(rank, suit));
                }
            }

            cards.Add(new Card(Rank.Joker, Suit.None));
            cards.Add(new Card(Rank.Joker, Suit.None));
        }

        // Fisher=Yates: every possible order is equally likely
        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }

        // The end of the list is the top of the deck; removing the last item is instant
        public Card Draw()
        {
            if (cards.Count == 0)
            {
                throw new InvalidOperationException("Tried to draw from an empty deck.");
            }

            int last = cards.Count - 1;
            Card top = cards[last];
            cards.RemoveAt(last);
            return top;
        }

        public void ResetAndShuffle()
        {
            Reset();
            Shuffle();
        }
    }
}