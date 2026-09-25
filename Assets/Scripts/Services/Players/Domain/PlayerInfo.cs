using MmoShared.Messages.Login.Domain;
using MmoShared.Messages.Players.Domain;
using UniRx;
using UnityEngine;

namespace Services.Players.Domain
{
    public class PlayerInfo
    {
        public long PlayerId { get; set; }
        public string CharacterName { get; set; }
        public AccountType AccountType { get; set; }
        
        public ReactiveProperty<Vector2Int> Position { get; }

        public PlayerInfo(PlayerDataDto playerDataDto)
        {
            PlayerId = playerDataDto.PlayerId;
            CharacterName = playerDataDto.CharacterName;
            AccountType = playerDataDto.AccountType;
            Position = new ReactiveProperty<Vector2Int>(new Vector2Int(playerDataDto.Position.x, playerDataDto.Position.y));
        }
    }
}