using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    public class MousePointer : MonoBehaviour
    {
        public static MousePointer Instance { get; private set; }
        private Controls _controls;

        public Vector2 DirectionAxis { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
            
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
            DirectionAxis = context.ReadValue<Vector2>();
        }
        
        public void SelectButton(GameObject go) 
        {
            FindObjectOfType<EventSystem>().SetSelectedGameObject(go);
        }
    }
}