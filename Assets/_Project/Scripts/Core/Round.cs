using System;
using System.Collections.Generic;

namespace LastTable.Core
{
    /// <summary>
    /// One hand at the table: the ante, the deal, and the pot.
    /// Betting and showdown build on top of this.
    /// </summary>
    public sealed class Round
    {
        private readonly List<Player> players;
        private readonly Deck deck;

        public Floor Floor { get; }
        public int DealerSeat { get; }
        public int Pot { get; private set; }
        
        // Ante the House paid on behalf of players who were short
        public int HouseCovered { get; private set; }

        public IReadOnlyList<Player> Players => players;
        public Player Dealer => players[DealerSeat];

        public Round(IReadOnlyList<Player> players, Floor floor, int dealerSeat, Deck deck)
        {
            if (players == null || players.Count < 3 || players.Count > 5)
            {
                throw new ArgumentException("A round needs 3 to 5 players.", nameof(players));
            }
            
            // Seats must match list positions, so players[seat] always finds the right person
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Seat != i)
                {
                    throw new ArgumentException(
                        $"{players[i].Name} is listed at {i} but sits in seat {players[i].Seat}.", nameof(players));
                }
            }

            if (dealerSeat < 0 || dealerSeat >= players.Count || players[dealerSeat].IsGhost)
            {
                throw new ArgumentOutOfRangeException(nameof(dealerSeat), "The Dealer must be a living player at the table.");
            }

            this.players = new List<Player>(players);
            Floor = floor ?? throw new ArgumentNullException(nameof(floor));
            this.deck = deck ?? throw new ArgumentNullException(nameof(deck));
            DealerSeat = dealerSeat;
        }

        /// <summary>
        /// Living players in table order: starting on the Dealer's left, ending with the Dealer.
        /// This is the order for the deal, the bet, and the Jacks and Queens.
        /// </summary>
        public List<Player> LivingFromDealersLeft()
        {
            List<Player> order = new List<Player>();

            for (int step = 1; step <= players.Count; step++)
            {
                // %wraps the count abck to seat 0 after the last seat
                Player p = players[(DealerSeat + step) % players.Count];

                if (!p.IsGhost)
                {
                    order.Add(p);
                }
            }

            return order;
        }
        
        //Step 1: every living player antes. Short players pay what they have; the House covers the rest.
        public void CollectAntes()
        {
            foreach (Player p in LivingFromDealersLeft())
            {
                int paid = p.Pay(Floor.Ante);
                HouseCovered += Floor.Ante - paid;
                Pot += Floor.Ante;
            }
        }
        
        //Step 2: a fresh 54-card deck, one hidden Fate card each, Dealer last
        public void Deal()
        {
            deck.ResetAndShuffle();

            foreach (Player p in LivingFromDealersLeft())
            {
                p.TakeCard(deck.Draw());
            }
        }

        /// <summary>
        /// The Witness (a 10) secretly sees the card of the nearest living player on their right.
        /// Returns that player, or null if this player isn't holding a 10.
        /// </summary>
        public Player WitnessTarget(Player witness)
        {
            if (witness == null || !witness.HasCard || witness.Hand.Rank != Rank.Ten)
            {
                return null;
            }

            for (int step = 1; step < players.Count; step++)
            {
                //Moving right means counting seats down; adding the count first keeps it from going negative
                Player p = players[(witness.Seat - step + players.Count) % players.Count];

                if (!p.IsGhost)
                {
                    return p;
                }
            }

            return null;
        }
        
        // Clears every hand and turns anyone left at 0 years into a ghost
        public void End()
        {
            foreach (Player p in players)
            {
                p.EndRound();
            }
        }
        
        // The marker passes left to the next living player
        public int NextDealerSeat()
        {
            List<Player> living = LivingFromDealersLeft();

            if (living.Count == 0)
            {
                throw new InvalidOperationException("No living players are left at the table.");
            }

            return living[0].Seat;
        }
    }
}
