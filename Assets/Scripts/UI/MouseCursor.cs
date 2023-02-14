using System;
using UnityEngine;


namespace UI
{
    public class MouseCursor : MonoBehaviour
    {
        private MouseNavigation _mouse;
       
    
        void Start() {
            Cursor.visible = false;
            _mouse = transform.parent.GetComponent<MouseNavigation>();
        }
        
        void Update() {
            transform.position = _mouse.DirectionAxis;
        }
    }
}
