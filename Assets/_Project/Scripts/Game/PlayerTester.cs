using System.Collections.Generic;
using UnityEngine;
using LastTable.Core;

namespace LastTable.Game
{
    /// <summary>
    /// Debug harness: seats four guests, antes and deals a Lobby hand,
    /// then sends one All-In guest to the House.
    /// </summary>
    public class PlayerTester : MonoBehaviour
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

            Floor floor = Climb.First;
            Deck deck = new Deck(rng);
            deck.Shuffle();
            int pot = 0;

            foreach (Player p in players)
            {
                pot += p.Pay(floor.Ante);
                p.TakeCard(deck.Draw());
                Debug.Log($"{p} antes {floor.Ante} and is dealt {p.Hand}");
            }

            Debug.Log($"Pot: {pot} Years");

            // Ben folds; Cal pushes every Year he has
            players[1].Fold();
            pot += players[2].Pay(players[2].Years);
            Debug.Log($"Ben folds. Cal goes All-In. Pot: {pot} Years");

            // For this test, Ana wins the pot
            players[0].Receive(pot);

            foreach (Player p in players)
            {
                p.EndRound();
                Debug.Log(p);
            }
        }
    }
}