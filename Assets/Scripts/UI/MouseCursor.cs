using System;
using UnityEngine;


namespace UI
{
    public class MouseCursor : MonoBehaviour
    {
        private MousePointer _mouse;


        private void Start() 
        {
            Cursor.visible = false;
            _mouse = transform.parent.GetComponent<MousePointer>();
        }

        private void Update() 
        {
            transform.position = _mouse.DirectionAxis;
        }
    }
}
