using UnityEngine.InputSystem;
using System.Collections;
using AnimationHandler;
using Dialogue;
using static Controls;
using SceneHandler;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: thoughts: Movement start/stop physics

//-------------------------------------------------------------------------------------------------------------------------
// Originally, this task comes from SAE Diploma (Games Programming) and is now being further developed.
// The purpose is to develop this class as a 2D-Controller Prototype with different Features and Options
// to de-/select and use it as a base(-blueprint) for new projects/prototypes.
//-------------------------------------------------------------------------------------------------------------------------

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(AnimationController), typeof(Collision))]
    public class PlayerController2D : MonoBehaviour, IGameplayActions
    {
        #region Feature Modes

        // Features to enable/disable in unity inspector
        [Header("MULTI-JUMP MODE")] [Space]
        [Tooltip("If the Checkbox is checked Multi-Jump is on. And you can jump (x)times when in the air.")]
        [SerializeField] private bool multiJump;

        [Tooltip("The amount of jumps when in air and Multi-Jump is enabled (2 or 3).")] 
        [Range(2, 3)] [SerializeField] private int multiJumps;
    
        [Space]
    
        [Header("WALL-FEATURES MODES")] [Space]
        [Tooltip("If the Checkbox is checked Wall Jump is on. If it�s unchecked it is not possible to jump off a wall.")]
        [SerializeField] private bool wallJump = true;

        [Tooltip("If the Checkbox is checked Wall Sliding is on. If it�s unchecked it is not possible to slide down a wall.")]
        [SerializeField] private bool wallSlide = true;

        #endregion

        #region Stats
        [Space]
    
        [Header("STATS")] [Space]
        [Tooltip("The movement-speed value to in/decrease the velocity")] 
        [Range(0f, 10f)] [SerializeField] private float moveSpeed;
        [Range(0f, 30f)] [SerializeField] private float runSpeed;

        [Tooltip("The force value to to in/decrease the jump-height.")] 
        [Range(0f, 10f)] [SerializeField] private float jumpForce;
        [Range(0f, 10f)] [SerializeField] private float wallJumpDivider = 1.5f;
        
        [Tooltip("The force value to to in/decrease the dash-force.")] 
        [Range(0f, 40f)] [SerializeField] private float dashForce = 14f;
        [Range(0f, 3f)] [SerializeField] private float dashDuration = .5f;
        [Range(0f, 3f)] [SerializeField] private float dashCooldown = .5f;
    
        [Space]
    
        [Tooltip("The possible amount of time in air when jumping.")] 
        [Range(0f, 0.5f)] [SerializeField] private float jumpTime;

        [Tooltip("The possible amount of time to jump when leaving the platform.")] 
        [Range(0f, 0.4f)] [SerializeField] private float coyoteTime;
    
        [Space]
    
        [Tooltip("The lerp value to in-/decrease the 'jump-feeling' off a wall.")] 
        [Range(0f, 10f)] [SerializeField] private float wallJumpLerp;

        [Tooltip("The slide speed value to in-/decrease the slide velocity at a wall.")] 
        [Range(0f, 4f)] [SerializeField] private float wallSlideSpeed;
        [Range(0f, 4f)] [SerializeField] private float wallClimbSpeed = 4f;
    
        [Space]
    
        [Tooltip("The gravity multiplier when the char y-velocity is > 0.")]
        [Range(0f, 10f)] [SerializeField] private float fallMultiplier = 2.5f;

        [Tooltip("The gravity multiplier when the char y-velocity is < 0.")] 
        [Range(0f, 10f)] [SerializeField] private float lowJumpMultiplier = 2f;

        #endregion

        #region Fields

        // Constants
        private const float JUMP_BUFFER_TIME = 0.2f;
        private const float SPEED_THROTTLE = 0.8f;
        
        // Components and classes
        private Controls _controls;
        private Collision _coll;
        private CinemachineShake _cinemachine;

        // Variables
        private Vector2[] _dashDirections;
        private Vector2 _dashDirection;
        private float _jumpBufferCounter;
        private float _coyoteTimeCounter;
        private float _jumpLengthCounter;
        private float _runSpeedValue;
        private float _velocityChange;
        private int _jumpCounter;

        // Bools
       
        private bool _wallJumped;
        private bool _canDash = true;
        private bool _facingRight = true;
        private bool _dashStarted;
        private bool _isGrabbing;
        private bool _canJump;

        #endregion
        
        #region Properties
        
        public Rigidbody2D Rigid { get; private set; }
        public InputAction JumpAction { get; private set; }
        public float InputX { get; private set; }
        public float InputY { get; private set; }
        public bool IsRunning { get; private set; }
        public bool Wallsliding { get; set; }
        public bool IsDashing { get; private set; }
        public bool IsInteracting { get; private set; }

        #endregion

        #region Events

        private void Awake()
        {
            _dashDirections = new[]
            {
                Vector2.up, Vector2.down, Vector2.left, Vector2.right, (Vector2.up + Vector2.left).normalized,
                (Vector2.up + Vector2.right).normalized, (Vector2.down + Vector2.left).normalized,
                (Vector2.down + Vector2.right).normalized
            };
            Rigid = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collision>();
            _controls = new Controls();
            _controls.Gameplay.SetCallbacks(this);
            JumpAction = _controls.Gameplay.Jump;
        }

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        private void Update()
        {
            _coll.FrictionChange(wallSlide);
            ResetterAndCounter();
        }

        private void FixedUpdate()
        {
            // stops the player from moving when in dialogue
            if (DialogueManager.Instance.OnDialogueIsActive)
                return;
            
            InAirBehavior();
            WallSlide();
            LongJump();
            Move();
            Dash();
            Grab();
            Flip();
        }

        #endregion

        #region Gameplay Action

        public void OnMove(InputAction.CallbackContext context)
        {
            InputX = context.ReadValue<Vector2>().x;
            InputY = context.ReadValue<Vector2>().y;
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (InputX != 0)
                IsRunning = context.action.IsPressed();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            JumpHandler(context);
        }
        
        public void OnDash(InputAction.CallbackContext context)
        {
            _dashStarted = context.performed;
            
            var dir = new Vector2(InputX, InputY);
            if (dir == Vector2.zero)
                _dashDirection =  transform.right;
            else
                _dashDirection = DashDirection(dir);
        }

        public void OnGrab(InputAction.CallbackContext context)
        {
            _isGrabbing = context.action.IsPressed();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            IsInteracting = context.started;
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.started && !GameManager.Instance.IsGamePaused)
                SceneLoader.Instance.LoadSceneAsync(SceneIndex.PauseMenu, LoadSceneMode.Additive);
        }
        
        #endregion

        #region Handler

        /// <summary>
        /// Handles the movement input.
        /// </summary>
        private void Move()
        {
            _runSpeedValue = runSpeed;
            _velocityChange = IsRunning ? _runSpeedValue : moveSpeed; // Set speed value
            
            if (!_wallJumped)
            {
                if (!_coll.IsGround()) _runSpeedValue *= SPEED_THROTTLE; // Throttle speed when in air
                else _runSpeedValue = runSpeed; // Reset speed when on ground
                
                Rigid.velocity = new Vector2(InputX * _velocityChange, Rigid.velocity.y);
            }
            else // In case of a wall jump the velocity is "lerped" to get a (immersive) feeling of less control.
                Rigid.velocity = Vector2.Lerp(Rigid.velocity, new Vector2(InputX * moveSpeed, Rigid.velocity.y)
                    ,wallJumpLerp * Time.deltaTime);
        }
        
        private void JumpHandler(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            MultiJump();
            WallJump();
        }

        // (Base-)Jump without parameters
        private void Jump()
        {
            Jump(Vector2.up);
        }

        // (Base-)Jump with parameters
        private void Jump(Vector2 dir)
        {
            Rigid.velocity = new Vector2(Rigid.velocity.x, 0);
            Rigid.velocity += dir * jumpForce;
            _canJump = false;
        }

        /// <summary>
        /// It does multiple jumps(x) and counts down the possible jumps(x) till zero.
        /// </summary>
        private void MultiJump()
        {
            if (!multiJump || _jumpCounter <= 0) return;
            Jump();
            _jumpCounter--;
        }

        // TODO: Better Wall-Jump experience
        /// <summary>
        /// It does a jump off a wall when this object has contact with a wall object and the move direction is pressed.
        /// </summary>
        private void WallJump()
        { 
            if (!JumpAction.IsPressed() || !wallJump || !_coll.IsWall() || _coll.IsGround()) return; 
            var wallDirection = _coll.IsRightWall() ? Vector2.left : Vector2.right;
            Jump(Vector2.up / wallJumpDivider + wallDirection / wallJumpDivider);
            Jump(Vector2.up / wallJumpDivider + wallDirection / wallJumpDivider);
            _wallJumped = true;
        }

        /// <summary>
        /// It does a higher jump when pressing the jump button and counts down a timer till zero.
        /// At the latest then the jump button is released.
        /// </summary>
        private void LongJump()
        {
            if (!JumpAction.IsPressed()) return;
            if (_jumpLengthCounter > 0f && _coyoteTimeCounter > 0f && _jumpBufferCounter > 0f && _canJump) 
            {
                Jump(); 
                _jumpBufferCounter = 0f;
            }

            _jumpLengthCounter -= Time.deltaTime;
        }
        
        private void Dash()
        {
            var ampIntensity = 2.9f;
            var freqIntensity = 2f;
            var shakeTime = .2f;
            if (_dashStarted && _canDash && !_coll.IsWall())
            {
                IsDashing = true;
                _canDash = false;
                StartCoroutine(StopDashing());
            }

            if (IsDashing)
            {
                Rigid.velocity = _dashDirection.normalized * dashForce;
                CinemachineShake.Instance.CameraShake(ampIntensity, freqIntensity, shakeTime);
            }
        }
        
        private IEnumerator StopDashing()
        {
            yield return new WaitForSeconds(dashDuration);
            Rigid.velocity = Vector2.zero;
            IsDashing = false;
            yield return new WaitForSeconds(dashCooldown);
            _canDash = true;
        }

        private Vector2 DashDirection(Vector2 vector)
        {
            var maxDot = Mathf.NegativeInfinity;
            var result = Vector2.zero;

            foreach (var direction in _dashDirections)
            {
                var product = Vector2.Dot(vector, direction);
                if (product > maxDot)
                {
                    result = direction;
                    maxDot = product;
                }
            }

            return result;
        }

        private void Grab()
        {
            if (InputX != 0 && _coll.IsWall() && _isGrabbing)
            {
                Rigid.velocity = new Vector2(Rigid.velocity.x, InputY * wallClimbSpeed);
            }
        }
    
        #endregion

        #region Physics Bahavior
        
        /// <summary>
        /// Handles the gravity in air for more immersive experience.
        /// </summary>
        private void InAirBehavior() // More immersive jump experience. Source: BetterJump (Youtube)
        {
            if (Wallsliding && _coll.IsGround() && _coll.IsWall()) return;
            if (Rigid.velocity.y < 0f)
            {
                Rigid.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // Fall faster
            }
            else if (Rigid.velocity.y > 0f && !JumpAction.IsPressed())
            {
                Rigid.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // Jump higher
            }
        }
    
        /// <summary>
        /// It does a down-slide at a wall when this object has contact with a wall object
        /// </summary>
        private void WallSlide()
        {
            //const float maxClampValue = 0;
            if (InputX != 0 && wallSlide && _coll.IsWall() && !_coll.IsGround())
            {
                //_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, -wallSlideSpeed, float.MaxValue));
                Wallsliding = true;
            }
            else
                Wallsliding = false;
        }

        /// <summary>
        /// Flips the gameobject to the direction it is moving.
        /// </summary>
        /// <param name="inputX">float</param>
        private void Flip()
        {
            if ((!(InputX > 0) || _facingRight) && 
                (!(InputX < 0) || !_facingRight)) return;
            _facingRight = !_facingRight;
            transform.Rotate(Vector3.up * 180);
        }
        
        #endregion

        #region Resetter / Counter
    
        /// <summary>
        /// Handles the variable values when certain events occur.
        /// </summary>
        private void ResetterAndCounter()
        {
            // Prevent double jumps
            if (JumpAction.WasReleasedThisFrame())
                _coyoteTimeCounter = 0f;

            if (_controls.Gameplay.Run.WasReleasedThisFrame())
                IsRunning = false;
            
            if (!JumpAction.IsPressed() && _coll.IsGround())
            {
                _jumpBufferCounter -= Time.deltaTime; 
                _coyoteTimeCounter = coyoteTime;
                _jumpLengthCounter = jumpTime; 
                _jumpCounter = multiJumps;
            
                Wallsliding = false;
                _wallJumped = false;
                _canJump = true;
            }
            else 
            {
                // If the player is not on ground the timer decreases
                _coyoteTimeCounter -= Time.deltaTime;
                _jumpBufferCounter = JUMP_BUFFER_TIME;
            }
        }
    
        #endregion
    }
}