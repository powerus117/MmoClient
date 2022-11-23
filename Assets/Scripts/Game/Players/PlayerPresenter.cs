using DG.Tweening;
using Game.Grid;
using MmoShared.Messages.Login.Domain;
using TMPro;
using UniRx;
using UnityEngine;

namespace Game.Players
{
    public class PlayerPresenter : MonoBehaviour
    {
        [SerializeField]
        private Transform _playerVisual;

        [SerializeField]
        private MeshRenderer _coloredMesh;

        [SerializeField]
        private TMP_Text _usernameText;
        
        private PlayerModel _model;

        private Sequence _moveSequence;
        private readonly CompositeDisposable _modelSubscriptions = new CompositeDisposable();

        public void Initialize(PlayerModel model)
        {
            _model = model;

            _usernameText.text = model.PlayerInfo.UserInfo.Username;

            if (model.PlayerInfo.UserInfo.AccountType == AccountType.Admin)
            {
                _usernameText.color = Color.green;
            }
            
            _model.PlayerInfo.Position.Subscribe(OnPositionChanged).AddTo(_modelSubscriptions);
            _coloredMesh.material.color = _model.PlayerInfo.Color;
        }

        private void OnPositionChanged(Vector2Int position)
        {
            Vector3 targetPosition = WorldGrid.GetPosition(position);
            Vector3 direction = targetPosition - transform.position;
            float angle = Vector3.Angle(Vector3.forward, direction);

            if (Vector3.Cross(Vector3.forward, direction).y < 0)
            {
                angle = -angle;
            }
            
            _moveSequence?.Kill();
            _moveSequence = DOTween.Sequence();
            _moveSequence.Join(transform.DOMove(targetPosition, 1f));
            _moveSequence.Join(_playerVisual.DOLocalRotate(new Vector3(0, angle, 0), 1f));
        }
    }
}