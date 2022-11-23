using MmoShared.Messages.Login.Domain;
using UniRx;
using UnityEngine;

namespace Services.Players.Domain
{
    public class PlayerInfo
    {
        public UserInfo UserInfo { get; }
        public Color Color { get; }
        public ReactiveProperty<Vector2Int> Position { get; }

        public PlayerInfo(UserInfo userInfo, Vector2Int position, string htmlColor)
        {
            UserInfo = userInfo;
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