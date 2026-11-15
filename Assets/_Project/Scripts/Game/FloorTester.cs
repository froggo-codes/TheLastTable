using UnityEngine;
using LastTable.Core;

namespace LastTable.Game
{
    /// <summary>
    /// Debug harness: walks the Climb and rolls each floor's dice once.
    /// </summary>
    public class FloorTester : MonoBehaviour
    {
        private void Start()
        {
            System.Random rng = new System.Random();

            foreach (Floor floor in Climb.Floors)
            {
                DicePair pair = new DicePair(floor.DieSides, rng);
                pair.RollBoth();
                Debug.Log($"{floor} -> rolled {pair}");
            }

            Debug.Log($"Top floor is {Climb.Top.Name}. Above it: the Exit.");
        }
    }
}

