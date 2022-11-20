using Services.Players.Domain;
using UniRx;
using UnityEngine;

namespace Services.Players
{
    public interface IPlayerService
    {
        IReadOnlyReactiveDictionary<ulong, PlayerInfo> Players { get; }
        void MoveToPosition(Vector2Int position);
    }
}