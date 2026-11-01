using UnityEngine;

namespace ObjectSystem.StationSystem.InventorySystem
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObjects/ItemData")]
    public class ItemData : ScriptableObject
    {
        public int id;
        public Sprite sprite;
    }
}
