using UnityEngine;
using LastTable.Core;

namespace LastTable.Game
{
    /// <summary>
    /// Debug harness: builds and shuffles a deck, then logs a few draws.
    /// Attach to an empty GameObject and press play.
    /// </summary>
    public class DeckTester : MonoBehaviour
    {
        [Tooltip("0 = new shuffle every run. Any other value repeats the same shuffle.")] 
        [SerializeField] private int seed = 0;
        [SerializeField] private int cardsToDraw = 5;

        private void Start()
        {
            // System.Random, not UnityEngine.random, so Core stays Engine-independent
            System.Random rng = seed == 0 ? new System.Random() : new System.Random(seed);

            Deck deck = new Deck(rng);
            Debug.Log($"New deck built: {deck.Count} cards.");

            deck.Shuffle();

            for (int i = 0; i < cardsToDraw; i++)
            {
                Card card = deck.Draw();
                Debug.Log($"Drew {card} | {card.Title} | worth {card.BaseValue}");
            }

            Debug.Log($"Cards left in deck: {deck.Count}");
        }
    }
}