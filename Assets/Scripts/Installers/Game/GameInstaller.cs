using Game.Players;
using Game.Players.Controller;
using UnityEngine;
using Zenject;

namespace Installers.Game
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private PlayerManager _playerManager;

        [SerializeField]
        private PlayerController _playerController;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerManager>().FromInstance(_playerManager).AsSingle();
            
            Container.Bind().FromInstance(_playerController);
        }
    }
}