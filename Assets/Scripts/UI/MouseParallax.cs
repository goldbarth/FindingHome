using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class MouseParallax : MonoBehaviour {
    
        [Header("PARALLAX EFFECTS SETTINGS")]
        [Range(0f, 50f)] [SerializeField] private float modifier;
        private Vector3 _startPos;
        private Camera _mainCam;

        private void Start() {
            _startPos = transform.position;
            _mainCam = Camera.main;
        }

        private void Update() {
            ParallaxFx();
        }

        // Get the mouse position from screen space and turn it into viewport space.
        // In combination with the modifier the camera position moves slightly with the mouse position.
        // "Et Voilá. Der Parallax-Effekt ist da."
        private void ParallaxFx() {
            var pos = _mainCam.ScreenToViewportPoint(Mouse.current.position.ReadValue());
            var posX = Mathf.Lerp(transform.position.x, _startPos.x + (pos.x * modifier), 2f * Time.deltaTime);
            var posY = Mathf.Lerp(transform.position.y, _startPos.y + (pos.y * modifier), 2f * Time.deltaTime);
            
            transform.position = new Vector3(posX, posY, 0);
            //pos.z = 0;
            //var position = new Vector3(_startPos.x + (pos.x * modifier), _startPos.y + (pos.y * modifier), 0);
            //transform.position = position;
        }
    }
}
