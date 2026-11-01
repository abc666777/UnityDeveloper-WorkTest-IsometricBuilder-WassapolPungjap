using System.Collections.Generic;
using UnityEngine;

namespace ObjectSystem.StationSystem.InventorySystem
{
    public static class Inventory
    {
        public static Dictionary<int, int> ItemAmount = new Dictionary<int, int>();

        public static void Add(int itemId, int amount)
        {
            if (!ItemAmount.ContainsKey(itemId))
                ItemAmount.Add(itemId, amount);
            else
                ItemAmount[itemId] += amount;
        }

        public static void Remove(int itemId, int amount)
        {
            if (!ItemAmount.ContainsKey(itemId))
            {
                Debug.LogError("ITEM IS NOT PRESENT");
                return;
            }

            if (ItemAmount[itemId] < amount)
            {
                Debug.LogError("NOT ENOUGH ITEM");
                return;
            }
            
            ItemAmount[itemId] -= amount;
        }

        public static int GetAmount(int itemId)
        {
            return !ItemAmount.ContainsKey(itemId) ? 0 : ItemAmount[itemId];
        }
    }
}
