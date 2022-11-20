using System;
using Game.Grid;
using UniRx;
using UnityEngine;

namespace Game.Players
{
    public class PlayerPresenter : MonoBehaviour
    {
        private PlayerModel _model;
        private readonly CompositeDisposable _modelSubscriptions = new CompositeDisposable();

        public void Initialize(PlayerModel model)
        {
            _model = model;

            _model.Position.Subscribe(OnPositionChanged).AddTo(_modelSubscriptions);
        }

        private void OnPositionChanged(Vector2Int position)
        {
            transform.position = WorldGrid.GetPosition(position);
        }
    }
}