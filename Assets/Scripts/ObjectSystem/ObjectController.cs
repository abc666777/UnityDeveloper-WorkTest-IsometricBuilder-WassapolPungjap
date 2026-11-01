using UnityEngine;

namespace ObjectSystem
{
    public class ObjectController : MonoBehaviour
    {
        [HideInInspector] public FurnitureData furnitureData;
        [HideInInspector] public Vector2Int originalGridPosition;
        [HideInInspector] public bool isFlip;

        public Vector2Int Size()
        {
            return isFlip ? new Vector2Int(furnitureData.size.y, furnitureData.size.x) : furnitureData.size;
        }
    }
}
