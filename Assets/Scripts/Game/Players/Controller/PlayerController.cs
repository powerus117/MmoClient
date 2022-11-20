using System;
using Game.Grid;
using Services.Players;
using UnityEngine;
using Zenject;

namespace Game.Players.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        
        [Inject]
        private IPlayerService _playerService;
        
        private Plane _groundPlane;

        private void Awake()
        {
            _groundPlane = new Plane(Vector3.up, Vector3.zero);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (_groundPlane.Raycast(ray, out float hitDistance))
                {
                    Vector3 hitPosition = ray.GetPoint(hitDistance);
                    Vector2Int gridPosition = WorldGrid.GetPosition(hitPosition);
                    
                    _playerService.MoveToPosition(gridPosition);
                }
            }
        }
    }
}