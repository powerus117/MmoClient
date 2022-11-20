﻿using System;
using Core.Connection.Messages;
using Core.Helpers;
using MmoShared.Messages.Players;
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
            _messageReceiver.Subscribe<PlayerMovedSync>(OnPlayerMovedSync);
        }

        public void Dispose()
        {
            _messageReceiver.Unsubscribe<LoadedSync>(OnLoadedSync);
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
                _players.Add(player.Key, new PlayerInfo(player.Value.Position.ToVector2Int()));
            }
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