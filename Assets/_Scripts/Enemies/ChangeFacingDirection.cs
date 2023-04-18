using System;
using UnityEngine;

namespace Enemies
{
    public class ChangeFacingDirection : MonoBehaviour
    {
        private Transform _transform;
        private Vector3 _previousPosition;
        
        private void Awake()
        {
            _transform = transform;
            _previousPosition = _transform.position;
        }

        private void Update()
        {
            if (_transform.position == _previousPosition) return;
            var facingRight = _transform.position.x > _previousPosition.x;
            _previousPosition = _transform.position;
            
            _transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        }
    }
}