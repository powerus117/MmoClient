using UniRx;
using UnityEngine;

namespace Services.Players.Domain
{
    public class PlayerInfo
    {
        public Color Color { get; }
        public ReactiveProperty<Vector2Int> Position { get; }

        public PlayerInfo(Vector2Int position, string htmlColor)
        {
            htmlColor = "#" + htmlColor;
            if (ColorUtility.TryParseHtmlString(htmlColor, out var color))
            {
                Color = color;
            }
            else
            {
                Debug.LogError("Failed to convert color " + htmlColor);
                Color = Color.red;
            }
            
            Position = new ReactiveProperty<Vector2Int>(position);
        }
    }
}