using MmoServer.Core;
using UnityEngine;

namespace Core.Helpers
{
    public static class Vector2IExtensions
    {
        public static Vector2Int ToVector2Int(this Vector2I vector2I)
        {
            return new Vector2Int(vector2I.x, vector2I.y);
        }
        
        public static Vector2I ToVector2I(this Vector2Int vector2Int)
        {
            return new Vector2I(vector2Int.x, vector2Int.y);
        }
    }
}