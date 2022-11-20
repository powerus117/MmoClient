using UniRx;
using UnityEngine;

namespace Services.Players.Domain
{
    public class PlayerInfo
    {
        public ReactiveProperty<Vector2Int> Position { get; }

        public PlayerInfo(Vector2Int position)
        {
            Position = new ReactiveProperty<Vector2Int>(position);
        }
    }
}