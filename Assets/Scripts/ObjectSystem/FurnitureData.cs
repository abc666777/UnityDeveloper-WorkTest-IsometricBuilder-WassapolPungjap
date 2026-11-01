using GridSystem;
using UnityEngine;

namespace ObjectSystem
{
    [CreateAssetMenu(fileName = "FurnitureData", menuName = "ScriptableObjects/FurnitureData")]
    public class FurnitureData : ScriptableObject
    {
        public int id;
        public GameObject prefab;
        public Vector2Int size;
        public bool isUnique;
        public GridType[] allowedGridType;
    }
}
