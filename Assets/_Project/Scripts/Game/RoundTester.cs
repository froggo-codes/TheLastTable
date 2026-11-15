using System.Collections.Generic;
using UnityEngine;
using LastTable.Core;

namespace LastTable.Game
{
    /// <summary>
    /// Debug harness: plays two rounds of antes and deals. Round two shows
    /// the marker skipping a Ghost and the House covering a short ante.
    /// </summary>
    public class RoundTester : MonoBehaviour
    {
        private void Start()
        {
            System.Random rng = new System.Random();
            string[] names = { "Ana", "Ben", "Cal", "Dee" };
            int startingYears = Player.StartingYearsFor(names.Length);

            List<Player> players = new List<Player>();
            for (int i = 0; i < names.Length; i++)
            {
                players.Add(new Player(names[i], i, startingYears));
            }

            Deck deck = new Deck(rng);

            // Round one: the Lobby, Ana deals
            Round first = new Round(players, Climb.Get(1), 0, deck);
            PlayRound(first);
            first.End();

            // Between rounds: Ben loses everything, Dee is down to 1 Year
            players[1].Pay(players[1].Years);
            players[1].EndRound();
            players[3].Pay(players[3].Years - 1);

            // Round two: the Bar, the marker passes left
            int nextDealer = first.NextDealerSeat();
            Round second = new Round(players, Climb.Get(2), nextDealer, deck);
            PlayRound(second);
            second.End();
        }

        private void PlayRound(Round round)
        {
            Debug.Log($"--- {round.Floor.Name}: {round.Dealer.Name} deals ---");

            round.CollectAntes();
            Debug.Log($"Antes in. Pot: {round.Pot} Years (House covered {round.HouseCovered})");

            round.Deal();
            foreach (Player p in round.LivingFromDealersLeft())
            {
                Debug.Log($"{p} holds {p.Hand}");

                Player seen = round.WitnessTarget(p);
                if (seen != null)
                {
                    Debug.Log($"{p.Name} is the Witness and secretly sees {seen.Name}'s {seen.Hand}");
                }
            }
        }
    }
}