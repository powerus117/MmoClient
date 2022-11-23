using UnityEngine;

namespace Core.Generic
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (_camera != null)
            {
                transform.LookAt(_camera.transform);
            }
        }
    }
}