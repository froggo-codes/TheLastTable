using System;
using System.Collections.Generic;

namespace LastTable.Core
{
    public enum BetMove
    {
        Check,
        Call,
        Raise,
        Fold,
        AllIn
    }

    /// <summary>
    /// The betting lap: one turn each from the Dealer's left, Dealer last.
    /// Then anyone below the top bet must Call, Fold, or go All-In if they're short.
    /// </summary>
    public sealed class BettingRound
    {
        private readonly Round round;
        private readonly Dictionary<Player, int> bets = new Dictionary<Player, int>();
        private readonly Queue<Player> toAct = new Queue<Player>();
        private bool lapDone;
        
        // The highest total anyone has bet this round (antes not included)
        public int TopBet { get; private set; }
        
        // Whose turn it is, or null when betting is over
        public Player CurrentPlayer => toAct.Count > 0 ? toAct.Peek() : null;
        public bool IsComplete => CurrentPlayer == null;

        public BettingRound(Round round)
        {
            this.round = round ?? throw new ArgumentNullException(nameof(round));

            foreach (Player p in round.LivingFromDealersLeft())
            {
                if (!p.IsInRound)
                {
                    continue;
                }

                bets[p] = 0;
                
                // A player who spent their last Year on the ante is already All-In: no turn
                if (p.Years > 0)
                {
                    toAct.Enqueue(p);
                }
            }

            EndEarlyIfOnePlayerLeft();
        }

        public int BetBy(Player player)
        {
            return bets.TryGetValue(player, out int bet) ? bet : 0;
        }

        public int OwedBy(Player player)
        {
            return TopBet - BetBy(player);
        }
        
        // Everyone who hasn't folded, in table order
        public List<Player> PlayersStillIn()
        {
            List<Player> stillIn = new List<Player>();

            foreach (Player p in round.LivingFromDealersLeft())
            {
                if (p.IsInRound)
                {
                    stillIn.Add(p);
                }
            }

            return stillIn;
        }

        public bool IsLegal(BetMove move, int raiseBy = 0)
        {
            Player p = CurrentPlayer;
            if (p == null)
            {
                return false;
            }

            int owed = OwedBy(p);

            switch (move)
            {
                case BetMove.Check:
                    return owed == 0;
                case BetMove.Call:
                    return owed > 0 && p.Years >= owed;
                case BetMove.Raise:
                    // Raises only happen in the first lap, so no one can raise twice.
                    return !lapDone && raiseBy >= 1 && raiseBy <= round.Floor.MaxRaise && p.Years >= owed + raiseBy;
                case BetMove.Fold:
                    return true;
                case BetMove.AllIn:
                    // All-in is for a player who can't cover the call
                    return owed > 0 && p.Years < owed;
                default:
                    return false;
            }
        }
        // What the bet bar lights up for the current player
        public List<BetMove> LegalMoves()
        {
            List<BetMove> moves = new List<BetMove>();

            foreach (BetMove move in Enum.GetValues(typeof(BetMove)))
            {
                if (IsLegal(move, 1))
                {
                    moves.Add(move);
                }
            }

            return moves;
        }

        public void Act(BetMove move, int raiseBy = 0)
        {
            Player p = CurrentPlayer;

            if (!IsLegal(move, raiseBy))
            {
                string who = p == null ? "Nobody" : p.Name;
                throw new InvalidOperationException($"{who} can't {move} right now.");
            }

            int owed = OwedBy(p);

            switch (move)
            {
                case BetMove.Check:
                    break;
                case BetMove.Call:
                    PutIn(p, owed);
                    break;
                case BetMove.Raise:
                    PutIn(p, owed + raiseBy);
                    TopBet += raiseBy;
                    break;
                case BetMove.Fold:
                    p.Fold();
                    break;
                case BetMove.AllIn:
                    PutIn(p, p.Years);
                    break;
            }

            toAct.Dequeue();
            Advance();
        }

        private void PutIn(Player player, int amount)
        {
            int paid = player.Pay(amount);
            bets[player] += paid;
            round.AddToPot(paid);
        }

        private void Advance()
        {
            if (EndEarlyIfOnePlayerLeft())
            {
                return;
            }

            if (toAct.Count > 0 || lapDone)
            {
                return;
            }
            
            // The lap is over: anyone below the top bet must now Call, Fold, or go All-In
            lapDone = true;

            foreach (Player p in PlayersStillIn())
            {
                if (p.Years > 0 && OwedBy(p) > 0)
                {
                    toAct.Enqueue(p);
                }
            }
        }
        
        // If everyone else folds, the last player in wins without a showdown
        private bool EndEarlyIfOnePlayerLeft()
        {
            if (PlayersStillIn().Count <= 1)
            {
                toAct.Clear();
                return true;
            }

            return false;
        }
    }
}