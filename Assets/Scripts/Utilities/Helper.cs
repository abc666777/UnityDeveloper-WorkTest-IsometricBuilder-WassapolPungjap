using UnityEngine;
using UnityEngine.Events;

namespace Utilities
{
    public static class Helper
    {
        public static UnityAction onCloseUi;

        public static Vector2 GridPositionToObjectPosition(this Vector2Int position)
        {
            return new Vector2((position.y - position.x) * 0.5f, (position.y + position.x) * 0.25f);
        }

        public static Vector2Int ObjectPositionToGridPosition(this Vector2 position)
        {
            var rawX = (position.y / 0.5f) - position.x;
            var rawY = position.x + (position.y / 0.5f);
            
            return new Vector2Int(Mathf.RoundToInt(rawX), Mathf.RoundToInt(rawY));
        }
    }
}
