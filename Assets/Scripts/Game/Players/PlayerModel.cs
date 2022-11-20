using UniRx;
using UnityEngine;

namespace Game.Players
{
    public class PlayerModel
    {
        public ReactiveProperty<Vector2Int> Position { get; }

        public PlayerModel(ReactiveProperty<Vector2Int> position)
        {
            Position = position;
        }
    }
}