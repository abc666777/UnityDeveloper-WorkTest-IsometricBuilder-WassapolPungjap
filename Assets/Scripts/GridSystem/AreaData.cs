using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace GridSystem
{
    [CreateAssetMenu(fileName = "Area Data", menuName = "ScriptableObjects/AreaData")]
    public class AreaData : ScriptableObject
    {
        [FormerlySerializedAs("zoneType")] public GridType gridType;
        public TileBase tileToGenerate;
        public Vector2Int startPos, endPos;
    }
}
