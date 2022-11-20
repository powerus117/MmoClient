using UnityEngine;

namespace Game.Grid
{
    public static class WorldGrid
    {
        public static Vector3 GetPosition(Vector2Int gridPosition)
        {
            return new Vector3(gridPosition.x + 0.5f, 0f, gridPosition.y + 0.5f);
        }
        
        public static Vector2Int GetPosition(Vector3 worldPosition)
        {
            return new Vector2Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.z));
        }
    }
}