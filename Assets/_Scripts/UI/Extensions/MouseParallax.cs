using UnityEngine.InputSystem;
using UnityEngine;

namespace UI.Extensions
{
    public class MouseParallax : MonoBehaviour 
    {
    
        [Header("PARALLAX EFFECTS SETTINGS")]
        [Range(0f, 2f)] [SerializeField] private float _modifier;
        
        [Space][Header("CAMERA")]
        [SerializeField] private Camera _menuCamera;
        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = transform.position;
            _menuCamera = Camera.main;
        }
        
        private void Update()
        {
            Parallax();
        }
        
        private void Parallax() 
        {
            var pos = _menuCamera.ScreenToViewportPoint(Mouse.current.position.ReadValue());
            var position = transform.position;
            var posX = Mathf.Lerp(position.x, _startPos.x + (pos.x * _modifier), 5f * Time.deltaTime);
            var posY = Mathf.Lerp(position.y, _startPos.y + (pos.y * _modifier), 5f * Time.deltaTime);
            
            position = new Vector3(posX, posY, position.z);
            transform.position = position;
        }
    }
}
