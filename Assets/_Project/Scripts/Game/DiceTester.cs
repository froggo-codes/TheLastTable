using UnityEngine;
using LastTable.Core;

namespace LastTable.Game
{
    /// <summary>
    /// Debug Harness: rolls a floor's dice, rerolls the low die, then rolls the Exit.
    /// </summary>
    public class DiceTester : MonoBehaviour
    {
        [Tooltip("Die size for the pair: 4, 6, 8, 10, 12, 20.")] 
        [SerializeField] private int sides = 6;
        
        [Tooltip("Years held at the Exit. You walk out on a d100 roll at or under this.")]
        [SerializeField] private int years = 37;

        private void Start()
        {
            System.Random rng = new System.Random();

            DicePair pair = new DicePair(sides, rng);
            pair.RollBoth();
            Debug.Log($"Rolled 2d{sides}: {pair}");

            DieSlot low = pair.LowestSlot;
            pair.Reroll(low);
            Debug.Log($"Rolled 2d{sides}: {pair}");

            int exitRoll = Dice.RollPecentile(rng);
            string fate = exitRoll <= years ? "walks out" : "is kept by the House";
            Debug.Log($"Exit with {years} Years: rolled {exitRoll}, {fate}.");
        }
    }
}