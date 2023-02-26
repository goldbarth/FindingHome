using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace UI
{
    public class MouseParallax : MonoBehaviour 
    {
    
        [Header("PARALLAX EFFECTS SETTINGS")]
        [Range(0f, 50f)] [SerializeField] private float modifier;
        
        [Space][Header("CAMERA")]
        [SerializeField] private Camera menuCamera;
        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = transform.position;
            menuCamera = Camera.main;
        }
        
        private void Update()
        {
            ParallaxFx();
        }
        
        private void ParallaxFx() 
        {
            var pos = menuCamera.ScreenToViewportPoint(Mouse.current.position.ReadValue());
            var position = transform.position;
            var posX = Mathf.Lerp(position.x, _startPos.x + (pos.x * modifier), 5f * Time.deltaTime);
            var posY = Mathf.Lerp(position.y, _startPos.y + (pos.y * modifier), 5f * Time.deltaTime);
            
            position = new Vector3(posX, posY, position.z);
            transform.position = position;
        }
    }
}
