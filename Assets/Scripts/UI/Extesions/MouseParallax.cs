using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class MouseParallax : MonoBehaviour 
    {
    
        [Header("PARALLAX EFFECTS SETTINGS")]
        [Range(0f, 50f)] [SerializeField] private float modifier;

        private Vector3 _startPos;
        private Camera _mainCam;

        private void Awake()
        {
            _startPos = transform.position;
            _mainCam = Camera.main;
        }
        
        private void Update()
        {
            ParallaxFx();
        }
        
        private void ParallaxFx() 
        {
            var pos = _mainCam.ScreenToViewportPoint(Mouse.current.position.ReadValue());
            var position = transform.position;
            var posX = Mathf.Lerp(position.x, _startPos.x + (pos.x * modifier), 5f * Time.deltaTime);
            var posY = Mathf.Lerp(position.y, _startPos.y + (pos.y * modifier), 5f * Time.deltaTime);
            
            position = new Vector3(posX, posY, position.z);
            transform.position = position;
            //pos.z = 0;
            //var position = new Vector3(_startPos.x + (pos.x * modifier), _startPos.y + (pos.y * modifier), 0);
            //transform.position = position;
        }
    }
}
