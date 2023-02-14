using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    public class MouseNavigation : MonoBehaviour
    {
        private Controls _controls;
        private Vector2 _pointerPos;
    
        public Vector2 DirectionAxis {
            get => _pointerPos;
            private set => _pointerPos = value;
        }

        private void Awake()
        {
            _controls = new Controls();
        }
        
        private void OnEnable()
        {
            _controls.UI.Enable();
            _controls.UI.Pointer.performed += OnPointerPerformed;
        }
        
        private void OnDisable()
        {
            _controls.UI.Disable();
            _controls.UI.Pointer.performed -= OnPointerPerformed;
        }

        private void OnPointerPerformed(InputAction.CallbackContext context)
        {
            _pointerPos = context.ReadValue<Vector2>();
        }
        
        public void SelectButton(GameObject go) {
            FindObjectOfType<EventSystem>().SetSelectedGameObject(go);
        }
    }
}