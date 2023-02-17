using static AnimationHandler.AnimationState;
using UnityEngine.InputSystem;
using System.Collections;
using AnimationHandler;
using static Controls;
using SceneHandler;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// TODO: thoughts: Movement start/stop physics

//-------------------------------------------------------------------------------------------------------------------------
// Originally, this task comes from SAE Diploma (Games Programming) and is now being further developed.
// The purpose is to develop this class as a 2D-Controller Prototype with different Features and Options
// to de-/select and use it as a base(-blueprint) for new projects/prototypes.
//-------------------------------------------------------------------------------------------------------------------------

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class PlayerController2D : MonoBehaviour, IGameplayActions
    {
        #region Feature Modes

        // Features to enable/disable in unity inspector
        [Header("MULTI-JUMP MODES")] [Space]
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
        [Range(0f, 4f)] [SerializeField] private float wallClimbSpeed = 2f;
    
        [Space]
    
        [Tooltip("The gravity multiplier when the char y-velocity is > 0.")]
        [Range(0f, 10f)] [SerializeField] private float fallMultiplier = 2.5f;

        [Tooltip("The gravity multiplier when the char y-velocity is < 0.")] 
        [Range(0f, 10f)] [SerializeField] private float lowJumpMultiplier = 2f;

        #endregion

        #region Fields

        // Components and classes
        private InputAction _jumpAction;
        private Controls _controls;
        private Rigidbody2D _rb;
        private Collision _coll;

        // Variables
        private readonly Vector2 _physicsGravity = new(0, -9.81f);
        private const float JUMP_BUFFER_TIME = 0.2f;
        private const float SPEED_THROTTLE = 0.8f;
        private const float LANDING_TIME = 0.2f;
        private float _inputX;
        private float _inputY;
        private float _jumpBufferCounter;
        private float _coyoteTimeCounter;
        private float _jumpLengthCounter;
        private float _runSpeedValue;
        private float _velocityChange;
        private int _jumpCounter;

        // Bools
        private bool _isRunning;
        private bool _wallJumped;
        private bool _wallsliding;
        private bool _isLanding;
        private bool _isDashing;
        private bool _canDash = true;
        private bool _facingRight = true;
        private bool _dashStarted;
        private bool _isGrabbing;
        private bool _isClimbing;
        private Vector2 dashDirection;

        #endregion

        #region Event Functions

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collision>();
            _controls = new Controls();
            _controls.Gameplay.SetCallbacks(this);
            _jumpAction = _controls.Gameplay.Jump;
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
            _coll.FrictionChange(wallSlide); // wallSlide TODO: wallSlide
            ResetterAndCounter();
            AnimationHandler();
        }

        private void FixedUpdate()
        {    
            InAirBehavior();
            WallSlide();
            LongJump();
            Move();
            Dash();
            Grab();
        }

        #endregion

        #region Gameplay Action

        public void OnMove(InputAction.CallbackContext context)
        {
            _inputX = context.ReadValue<Vector2>().x;
            _inputY = context.ReadValue<Vector2>().y;
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (_inputX != 0)
            {
                _isRunning = context.action.IsPressed();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            JumpHandler(context);
        }
        
        public void OnDash(InputAction.CallbackContext context)
        {
            _dashStarted = context.performed;
            dashDirection.y = _inputY;
            if (_inputX == 0f)
                dashDirection.x = transform.right.x;
            else
                dashDirection.x = _inputX;
        }
        
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.started && !GameManager.Instance.IsPaused)
            {
                SceneLoader.Instance.LoadSceneAsync(SceneIndex.PauseMenu, LoadSceneMode.Additive);
            }
        }
        
        #endregion

        #region Handler

        /// <summary>
        /// Handles the movement input.
        /// </summary>
        private void Move()
        {
            _runSpeedValue = runSpeed;
            _velocityChange = _isRunning ? _runSpeedValue : moveSpeed; // Set speed value
            
            if (!_wallJumped)
            {
                if (!_coll.IsGround()) _runSpeedValue *= SPEED_THROTTLE; // Throttle speed when in air
                else _runSpeedValue = runSpeed; // Reset speed when on ground
                
                _rb.velocity = new Vector2(_inputX * _velocityChange, _rb.velocity.y);
            }
            else // In case of a wall jump the velocity is "lerped" to get a (immersive) feeling of less control.
            {
                _rb.velocity = Vector2.Lerp(_rb.velocity, new Vector2(_inputX * _velocityChange, _rb.velocity.y)
                    ,wallJumpLerp * Time.deltaTime);
            }
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
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.velocity += dir * jumpForce;
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

        // TODO: Better Walljump experience
        /// <summary>
        /// It does a jump off a wall when this object has contact with a wall object and the move direction is pressed.
        /// </summary>
        private void WallJump()
        {
            if (!_jumpAction.IsPressed() || !wallJump || !_coll.IsWall() || _coll.IsGround()) return; 
            var wallDirection =
                _coll.IsRightWall()
                    ? Vector2.left
                    : Vector2.right;
            Jump(Vector2.up / 1.5f + wallDirection / 1.5f);
            _wallJumped = true;
        }

        /// <summary>
        /// It does a higher jump when pressing the jump button and counts down a timer till zero.
        /// At the latest then the jump button is released.
        /// </summary>
        private void LongJump()
        {
            if (!_jumpAction.IsPressed()) return;
            if (_jumpLengthCounter > 0f && _coyoteTimeCounter > 0f && _jumpBufferCounter > 0f) 
            {
                Jump(); 
                _jumpBufferCounter = 0f;
            }

            _jumpLengthCounter -= Time.deltaTime;
        }
        
        private void Dash()
        {
            if (dashDirection == Vector2.zero) return;
            if (_dashStarted && _canDash && !_coll.IsWall())
            {
                _isDashing = true;
                _canDash = false;
                StartCoroutine(StopDashing());
            }

            if (_isDashing)
                //_rb.AddForce(dir.normalized * dashForce, ForceMode2D.Impulse);
                _rb.velocity = dashDirection.normalized * dashForce;
        }

        private void Grab()
        {
            if (_inputX != 0 && _coll.IsWall() && !_coll.IsGround())
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _inputY * wallClimbSpeed);
            }
        }
    
        #endregion

        /// <summary>
        /// Handles the gravity in air for more immersive experience.
        /// </summary>
        private void InAirBehavior() // More immersive jump experience. Source: BetterJump (Youtube)
        {
            if (_wallsliding && _coll.IsGround() && _coll.IsWall()) return;
            if (_rb.velocity.y < 0f)
            {
                _rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // Fall faster
            }
            else if (_rb.velocity.y > 0f && !_jumpAction.IsPressed())
            {
                _rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // Jump higher
            }
        }
    
        /// <summary>
        /// It does a down-slide at a wall when this object has contact with a wall object
        /// </summary>
        private void WallSlide()
        {
            //const float maxClampValue = 0;
            if (_inputX != 0 && wallSlide && _coll.IsWall() && !_coll.IsGround())
            {
                //_rb.velocity = new Vector2(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, -wallSlideSpeed, float.MaxValue));
                _wallsliding = true;
            }
            else
                _wallsliding = false;
        }
        
        private void AnimationHandler()
        {
            Flip(_inputX);
            
            var onLandingCanceled = _coll.IsGround() && _jumpAction.IsPressed();
            if (_coll.IsGround() && !_isRunning && !_jumpAction.IsPressed() 
                && !_isLanding && !_isClimbing || onLandingCanceled)
                AnimationManager.Instance.SetAnimationState(_inputX != 0
                    ? player_walk
                    : player_idle);

            if (_coll.IsGround() && _isRunning)
                AnimationManager.Instance.SetAnimationState(player_run);
            
            if (_isDashing)
                AnimationManager.Instance.SetAnimationState(player_dash);

            if (_rb.velocity.y > 0 && !_isClimbing && !_coll.IsWall() && !_isDashing)
            {
                AnimationManager.Instance.SetAnimationState(player_jump);
            }
            else if (_rb.velocity.y < 0 && !_coll.IsNearGround() && !_coll.IsGround() 
                     && !_wallsliding && !_isClimbing && !_coll.IsWall() && !_isDashing)
            {
                AnimationManager.Instance.SetAnimationState(player_fall);
            }
            
            if (_rb.velocity.y < 0 && _coll.IsNearGround() && !_wallsliding || _isLanding)
                StartCoroutine(LandingAnimation());
                
            var isOnWall = _inputX != 0 && _coll.IsWall() && !_coll.IsGround();
            if (isOnWall && _inputY > 0)
            {
                AnimationManager.Instance.SetAnimationState(player_wallclimb);
                _wallsliding = false;
                _isClimbing = true;
            }
            else
                _isClimbing = false;
            if (isOnWall && _inputY == 0 && !_wallsliding && !_isClimbing)
                AnimationManager.Instance.SetAnimationState(player_wallgrab);

            if (isOnWall && _wallsliding && !_isClimbing)
                AnimationManager.Instance.SetAnimationState(player_wallslide);
        }

        private IEnumerator LandingAnimation()
        {
            _isLanding = true;
            AnimationManager.Instance.SetAnimationState(player_land);
            yield return new WaitForSeconds(LANDING_TIME);
            _isLanding = false;
        }
        
        private IEnumerator StopDashing()
        {
            yield return new WaitForSeconds(dashDuration);
            _rb.velocity = Vector2.zero;
            _isDashing = false;
            yield return new WaitForSeconds(dashCooldown);
            _canDash = true;
        }

        /// <summary>
        /// Flips the gameobject to the direction it is moving.
        /// </summary>
        /// <param name="moveInput">float</param>
        private void Flip(float moveInput)
        {
            if ((!(moveInput > 0) || _facingRight) && 
                (!(moveInput < 0) || !_facingRight)) return;
            _facingRight = !_facingRight;
            transform.Rotate(Vector3.up * 180);
        }

        #region Resetter / Counter
    
        /// <summary>
        /// Handles the variable values when certain events occur.
        /// </summary>
        private void ResetterAndCounter()
        {
            if (!_jumpAction.IsPressed() && _coll.IsGround())
            {
                _jumpBufferCounter -= Time.deltaTime; 
                _coyoteTimeCounter = coyoteTime;
                _jumpLengthCounter = jumpTime; 
                _jumpCounter = multiJumps;
            
                _wallsliding = false;
                _wallJumped = false;
            }
            else 
            {
                // If the player is not on ground the timer decreases
                _coyoteTimeCounter -= Time.deltaTime;
                _jumpBufferCounter = JUMP_BUFFER_TIME;
            }

            // Prevent double jumps
            if (_jumpAction.WasReleasedThisFrame())
                _coyoteTimeCounter = 0f; 
        }
    
        #endregion
    }
}