using UnityEngine;

namespace ObjectSystem.StationSystem.InventorySystem
{
    [CreateAssetMenu(fileName = "RecipeData", menuName = "ScriptableObjects/RecipeData")]
    public class RecipeData : ScriptableObject
    {
        public int id;
        public ItemData[] itemRequired;
        public int[] amountRequired;
        public ItemData itemOutput;
    }
}
