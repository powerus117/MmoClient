using System;
using Core.Connection.Messages;
using Core.Helpers;
using MmoShared.Messages.Players;
using MmoShared.Messages.Players.Movement;
using Services.Players.Domain;
using UniRx;
using UnityEngine;
using Zenject;
using IMessageReceiver = Core.Connection.Messages.IMessageReceiver;

namespace Services.Players
{
    public class PlayerService : IPlayerService, IInitializable, IDisposable
    {
        [Inject]
        private IMessageReceiver _messageReceiver;

        [Inject]
        private IMessageSender _messageSender;

        private ReactiveDictionary<ulong, PlayerInfo> _players = new ReactiveDictionary<ulong, PlayerInfo>();
        
        public IReadOnlyReactiveDictionary<ulong, PlayerInfo> Players => _players;

        public void Initialize()
        {
            _messageReceiver.Subscribe<LoadedSync>(OnLoadedSync);
            _messageReceiver.Subscribe<AddPlayerSync>(OnAddPlayerSync);
            _messageReceiver.Subscribe<RemovePlayerSync>(OnRemovePlayerSync);
            _messageReceiver.Subscribe<PlayerMovedSync>(OnPlayerMovedSync);
        }

        public void Dispose()
        {
            _messageReceiver.Unsubscribe<LoadedSync>(OnLoadedSync);
            _messageReceiver.Unsubscribe<AddPlayerSync>(OnAddPlayerSync);
            _messageReceiver.Unsubscribe<RemovePlayerSync>(OnRemovePlayerSync);
            _messageReceiver.Unsubscribe<PlayerMovedSync>(OnPlayerMovedSync);
        }

        public void MoveToPosition(Vector2Int position)
        {
            _messageSender.Send(new PlayerMoveNotify()
            {
                Position = position.ToVector2I()
            });
        }
        
        private void OnLoadedSync(LoadedSync sync)
        {
            _players.Clear();

            foreach (var player in sync.Players)
            {
                var playerData = player.Value;
                _players.Add(player.Key, new PlayerInfo(playerData.UserInfo, playerData.Position.ToVector2Int(), playerData.Color));
            }
        }
        
        private void OnAddPlayerSync(AddPlayerSync sync)
        {
            var playerData = sync.PlayerData;
            _players.Add(sync.PlayerData.UserInfo.UserId, new PlayerInfo(playerData.UserInfo, playerData.Position.ToVector2Int(), playerData.Color));
        }
        
        private void OnRemovePlayerSync(RemovePlayerSync sync)
        {
            _players.Remove(sync.UserId);
        }
        
        private void OnPlayerMovedSync(PlayerMovedSync sync)
        {
            if (_players.TryGetValue(sync.UserId, out var playerInfo))
            {
                playerInfo.Position.Value = sync.Position.ToVector2Int();
            }
        }
    }
}