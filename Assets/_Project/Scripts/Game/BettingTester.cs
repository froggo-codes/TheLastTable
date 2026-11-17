using System.Collections.Generic;
using UnityEngine;
using LastTable.Core;

namespace LastTable.Game
{
    /// <summary>
    /// Debug harness: a scripted betting round on the Bar that uses every move.
    /// </summary>
    public class BettingTester : MonoBehaviour
    {
        private void Start()
        {
            List<Player> players = new List<Player>
            {
                new Player("Ana", 0, 25),
                new Player("Ben", 1, 25),
                new Player("Cal", 2, 25),
                new Player("Dee", 3, 3)
            };

            Round round = new Round(players, Climb.Get(2), 0, new Deck(new System.Random()));
            round.CollectAntes();
            round.Deal();
            Debug.Log($"--- {round.Floor.Name}: antes in, pot {round.Pot} ---");

            BettingRound betting = new BettingRound(round);

            Play(betting, BetMove.Check);
            Play(betting, BetMove.Raise, 2);
            Play(betting, BetMove.AllIn);
            Play(betting, BetMove.Raise, 1);
            Play(betting, BetMove.Fold);
            Play(betting, BetMove.Call);

            Debug.Log($"Betting complete: {betting.IsComplete}. Pot: {round.Pot}");

            foreach (Player p in betting.PlayersStillIn())
            {
                Debug.Log($"Still in: {p} with {betting.BetBy(p)} bet");
            }
        }

        private void Play(BettingRound betting, BetMove move, int raiseBy = 0)
        {
            Player p = betting.CurrentPlayer;
            string options = string.Join(", ", betting.LegalMoves());

            betting.Act(move, raiseBy);

            string amount = move == BetMove.Raise ? $" {raiseBy}" : "";
            Debug.Log($"{p.Name} could {options} -> {move}{amount}. Top bet {betting.TopBet}, pot {betting.PlayersStillIn().Count} still in");
        }
    }
}