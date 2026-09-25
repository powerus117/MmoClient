using System;
using System.Collections.Generic;
using Game.Grid;
using Services.Login;
using Services.Players;
using Services.Players.Domain;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Players
{
    public class PlayerManager : MonoBehaviour, IInitializable, IDisposable
    {
        [SerializeField]
        private GameObject _playerPrefab;

        [SerializeField]
        private GameObject _ownPlayer;
        
        [Inject]
        private IPlayerService _playerService;

        [Inject]
        private ILoginService _loginService;

        private Dictionary<long, PlayerPresenter> _players = new();

        private readonly CompositeDisposable _serviceSubscriptions = new();

        public void Initialize()
        {
            _playerService.Players.ObserveReset().Subscribe(OnPlayersReset).AddTo(_serviceSubscriptions);
            _playerService.Players.ObserveReplace().Subscribe(OnPlayerReplaced).AddTo(_serviceSubscriptions);
            _playerService.Players.ObserveRemove().Subscribe(OnPlayerRemoved).AddTo(_serviceSubscriptions);
            _playerService.Players.ObserveAdd().Subscribe(OnPlayerAdded).AddTo(_serviceSubscriptions);
            
            foreach (var playerPair in _playerService.Players)
            {
                AddPlayer(playerPair.Key, playerPair.Value);
            }
        }
        
        public void Dispose()
        {
            _serviceSubscriptions.Dispose();
        }

        private void OnPlayersReset(Unit unit)
        {
            throw new NotImplementedException();
        }

        private void OnPlayerReplaced(DictionaryReplaceEvent<long, PlayerInfo> replaceEvent)
        {
            throw new NotImplementedException();
        }

        private void OnPlayerRemoved(DictionaryRemoveEvent<long, PlayerInfo> removeEvent)
        {
            if (_players.TryGetValue(removeEvent.Key, out var playerPresenter))
            {
                Destroy(playerPresenter.gameObject);
            }

            _players.Remove(removeEvent.Key);
        }

        private void OnPlayerAdded(DictionaryAddEvent<long, PlayerInfo> addEvent)
        {
            AddPlayer(addEvent.Key, addEvent.Value);
        }

        private void AddPlayer(long userId, PlayerInfo playerInfo)
        {
            if (userId != _loginService.PlayerDataDto.PlayerId)
            {
                var newPlayerObject = Instantiate(_playerPrefab, WorldGrid.GetPosition(playerInfo.Position.Value), Quaternion.identity);
                var newPlayer = newPlayerObject.GetComponent<PlayerPresenter>();
                newPlayer.Initialize(new PlayerModel(playerInfo));
                _players[userId] = newPlayer;
            }
            else
            {
                _ownPlayer.transform.position = WorldGrid.GetPosition(playerInfo.Position.Value);
                var newPlayer = _ownPlayer.GetComponent<PlayerPresenter>();
                newPlayer.Initialize(new PlayerModel(playerInfo));
                _players[userId] = newPlayer;
            }
        }
    }
}