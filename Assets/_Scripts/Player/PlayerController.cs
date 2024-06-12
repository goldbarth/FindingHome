using System;
using UnityEngine.InputSystem;
using System.Collections;
using Player.PlayerData;
using AnimationHandler;
using static Controls;
using UnityEngine;
using Dialogue;

//-------------------------------------------------------------------------------------------------------------------------
// Originally, this task comes from SAE Diploma (Games Programming) and is now being further developed.
// The purpose is to develop this class as a 2D-Controller Prototype with different Features and Options
// to de-/select and use it as a base(-blueprint) for new projects/prototypes.
//-------------------------------------------------------------------------------------------------------------------------

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collision))]
    public class PlayerController : MonoBehaviour, IGameplayActions
    {
        #region Feature Modes

        // Features to enable/disable in unity inspector
        [Header("MULTI-JUMP MODE")] [Space]
        [Tooltip("If the Checkbox is checked Multi-Jump is on. And you can jump (x)times when in the air.")]
        [SerializeField] private bool _multiJump;
        [Tooltip("The amount of jumps when in air and Multi-Jump is enabled (2 or 3).")] 
        [Range(2, 3)] [SerializeField] private int _multiJumps;
        
        [Tooltip("If the Checkbox is checked Dashing is on.")]
        [SerializeField] private bool _dashEnabled;


        [Header("WALL-FEATURES MODES")] [Space]
        [Tooltip("If the Checkbox is checked Wall Jump is on. If it�s unchecked it is not possible to jump off a wall.")]
        [SerializeField] private bool _wallJump = true;

        [Tooltip("If the Checkbox is checked Wall Sliding is on. If it�s unchecked it is not possible to slide down a wall.")]
        [SerializeField] private bool _wallSlide = true;

        #endregion

        #region Stats
        [Space]
    
        [Header("STATS")] [Space]
        [Tooltip("The movement-speed value to in/decrease the velocity")] 
        [Range(0f, 10f)] [SerializeField] private float _moveSpeed = 6.5f;
        [Range(0f, 30f)] [SerializeField] private float _runSpeed = 10f;

        [Tooltip("The force value to to in/decrease the jump-height.")] 
        [Range(0f, 10f)] [SerializeField] private float _jumpForce = 6.5f;
        [Range(0f, 10f)] [SerializeField] private float _wallJumpDivider = 1.5f;
        
        [Tooltip("The force value to to in/decrease the dash-force.")] 
        [Range(0f, 40f)] [SerializeField] private float _dashForce = 30f;
        [Range(0f, 3f)] [SerializeField] private float _dashDuration = .2f;
        [Range(0f, 3f)] [SerializeField] private float _dashCooldown = 2f;
    
        [Space]
    
        [Tooltip("The possible amount of time in air when jumping.")] 
        [Range(0f, 0.5f)] [SerializeField] private float _jumpTime = .1f;

        [Tooltip("The possible amount of time to jump when leaving the platform.")] 
        [Range(0f, 0.4f)] [SerializeField] private float _coyoteTime = .2f;
    
        [Space]
    
        [Tooltip("The lerp value to in-/decrease the 'jump-feeling' off a wall.")] 
        [Range(0f, 10f)] [SerializeField] private float _wallJumpLerp = 3.5f;

        [Tooltip("The slide speed value to in-/decrease the slide velocity at a wall.")] 
        [Range(0f, 4f)] [SerializeField] private float _wallSlideSpeed = 1.85f;
        [Range(0f, 4f)] [SerializeField] private float _wallClimbSpeed = 2f;
    
        [Space]
    
        [Tooltip("The gravity multiplier when the char is falling.")]
        [Range(0f, 10f)] [SerializeField] private float _fallMultiplier = 3.5f;

        [Tooltip("The gravity multiplier when the char is jumping till highest point in air.")] 
        [Range(0f, 10f)] [SerializeField] private float _lowJumpMultiplier = 1.7f;

        #endregion

        #region Fields

        // Constants
        private const float JumpBufferTime = 0.2f;
        private const float SpeedThrottle = 0.8f;
        
        // Components and classes
        private DialogueManager _dialogueManager;
        private CinemachineShake _cinemachine;
        private EatablesCount _eatablesCount;
        private Animator _animator;
        private Controls _controls;
        private Collision _coll;

        // Variables
        private Vector2[] _dashDirections;
        private Vector2 _dashDirection;
        private float _jumpBufferCounter;
        private float _coyoteTimeCounter;
        private float _jumpLengthCounter;
        private float _velocityChange;
        private float _runSpeedValue;

        // Bools
        private bool _facingRight = true;
        private bool _canDash = true;
        private bool _canMultiJump;
        private bool _dashStarted;
        private bool _wallJumped;
        private bool _isGrabbing;

        #endregion
        
        #region Properties
        
        public InputAction JumpAction { get; private set; }
        public Rigidbody2D Rigid { get; private set; }
        public int JumpCounter { get; private set; }
        public float InputY { get; private set; }
        public float InputX { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsDashing { get; private set; }
        public bool CanJump { get; private set; }
        public bool IsInteracting { get; private set; }
        public bool Wallsliding { get; set; }
        public bool CanMultiJump => _multiJump && JumpCounter > 0;
        public bool HasEatablesDecreased => _eatablesCount.HasEatableDecreased();
        public int GetEatablesCount => _eatablesCount.GetCount();

        #endregion
        
        // Getters
        public bool IsMultiJumpActive() => _multiJump;
        
        // Setters
        public void ActivateMultiJump() => _multiJump = true;

        #region Events

        private void Awake()
        {
            _dashDirections = new[]
            {
                Vector2.up, Vector2.down, Vector2.left, Vector2.right, (Vector2.up + Vector2.left).normalized,
                (Vector2.up + Vector2.right).normalized, (Vector2.down + Vector2.left).normalized,
                (Vector2.down + Vector2.right).normalized
            };
            _dialogueManager = FindObjectOfType<DialogueManager>();
            _eatablesCount = GetComponent<EatablesCount>();
            _animator = GetComponentInChildren<Animator>();
            Rigid = GetComponent<Rigidbody2D>();
            _coll = GetComponent<Collision>();
            
            _controls = new Controls();
            _controls.Gameplay.SetCallbacks(this);
            JumpAction = _controls.Gameplay.Jump;
        }

        private void Start()
        {
            GameManager.Instance.IsMenuActive = false;
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
            _coll.FrictionChange(_wallSlide);
            ResetterAndCounter();

            //TODO: declare a bool on a different place
            //if (_dialogueManager is not null && !_dialogueManager.IsInDialogue)
            //    _multiJump = true;
        }

        private void FixedUpdate()
        {

            if (GameManager.Instance.IsRespawning)
                return;
            
            //stops the player from floating above the ground or moving when in dialogue
            if ((_dialogueManager is not null && _dialogueManager.IsInDialogue) && 
                GameManager.Instance.IsGameStarted)
            {
                Rigid.velocity = Vector2.zero;
                _animator.SetBool("IsWalking", false);
                return;
            }
            
            AnimationControl();
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
            if(!_dashEnabled) return;
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
            IsInteracting = context.action.triggered;
        }

        #endregion

        #region Handler

        /// <summary>
        /// Handles the movement input.
        /// </summary>
        private void Move()
        {
            _runSpeedValue = _runSpeed;
            _velocityChange = IsRunning ? _runSpeedValue : _moveSpeed; // Set speed value
            
            if (!_wallJumped)
            {
                if (!_coll.IsGround()) _runSpeedValue *= SpeedThrottle; // Throttle speed when in air
                else _runSpeedValue = _runSpeed; // Reset speed when on ground
                
                Rigid.velocity = new Vector2(InputX * _velocityChange, Rigid.velocity.y);
            }
            else // In case of a wall jump the velocity is "lerped" to get a (immersive) feeling of less control.
                Rigid.velocity = Vector2.Lerp(Rigid.velocity, new Vector2(InputX * _moveSpeed, Rigid.velocity.y)
                    ,_wallJumpLerp * Time.deltaTime);
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
            var velocity = Rigid.velocity;
            velocity = new Vector2(velocity.x, 0);
            velocity += dir * _jumpForce;
            Rigid.velocity = velocity;
            CanJump = false;
        }

        /// <summary>
        /// It does multiple jumps(x) and counts down the possible jumps(x) till zero.
        /// </summary>
        private void MultiJump()
        {
            if (_dialogueManager != null)
                if (_dialogueManager.IsInDialogue) return;
            if (!_multiJump || JumpCounter <= 0) return;
            Jump();
            JumpCounter--;
        }

        // TODO: Better Wall-Jump experience
        /// <summary>
        /// It does a jump off a wall when this object has contact with a wall object and the move direction is pressed.
        /// </summary>
        private void WallJump()
        { 
            if (!JumpAction.IsPressed() || !_wallJump || !_coll.IsWall() || _coll.IsGround()) return; 
            var wallDirection = _coll.IsRightWall() ? Vector2.left : Vector2.right;
            Jump(Vector2.up / _wallJumpDivider + wallDirection / _wallJumpDivider);
            Jump(Vector2.up / _wallJumpDivider + wallDirection / _wallJumpDivider);
            _wallJumped = true;
        }

        /// <summary>
        /// It does a higher jump when pressing the jump button and counts down a timer till zero.
        /// At the latest then the jump button is released.
        /// </summary>
        private void LongJump()
        {
            if (!JumpAction.IsPressed()) return;
            if (_jumpLengthCounter > 0f && _coyoteTimeCounter > 0f && _jumpBufferCounter > 0f && CanJump) 
            {
                Jump(); 
                _jumpBufferCounter = 0f;
            }

            _jumpLengthCounter -= Time.deltaTime;
        }
        
        private void Dash()
        {
            if (_dashStarted && _canDash && !_coll.IsWall())
            {
                IsDashing = true;
                _canDash = false;
                StartCoroutine(StopDashing());
            }

            if (IsDashing)
            {
                Rigid.velocity = _dashDirection.normalized * _dashForce;
                CinemachineShake.Instance.CameraShake();
            }
        }
        
        private IEnumerator StopDashing()
        {
            yield return new WaitForSeconds(_dashDuration);
            Rigid.velocity = Vector2.zero;
            IsDashing = false;
            yield return new WaitForSeconds(_dashCooldown);
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
                Rigid.velocity = new Vector2(Rigid.velocity.x, InputY * _wallClimbSpeed);
            }
        }
    
        #endregion

        #region Physics Bahavior
        
        /// <summary>
        /// Handles the gravity in air for more immersive experience.
        /// </summary>
        private void InAirBehavior() // More immersive jump experience. Source: Cris: BetterJump (Youtube) *zwinkersmiley*
        {
            if (Wallsliding && _coll.IsGround() && _coll.IsWall()) return;
            if (Rigid.velocity.y < 0f)
            {
                Rigid.velocity += (_fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // Fall faster
            }
            else if (Rigid.velocity.y > 0f && !JumpAction.IsPressed())
            {
                Rigid.velocity += (_lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // Jump higher
            }
        }
    
        /// <summary>
        /// It does a down-slide at a wall when this object has contact with a wall object
        /// </summary>
        private void WallSlide()
        {
            //const float maxClampValue = 0;
            if (InputX != 0 && _wallSlide && _coll.IsWall() && !_coll.IsGround())
            { 
                Rigid.velocity = new Vector2(Rigid.velocity.x, Mathf.Clamp(Rigid.velocity.y, -_wallSlideSpeed, float.MaxValue));
                Wallsliding = true;
            }
            else
                Wallsliding = false;
        }
        
        private void AnimationControl()
        {
            if (_coll.IsGround() && !JumpAction.WasPressedThisFrame())
                _animator.SetBool("IsWalking", InputX != 0);
            
            if (_coll.IsGround() && JumpAction.IsPressed())
                _animator.SetBool("IsJumping", true);

            else if (Rigid.velocity.y < 0 && !_coll.IsGround())
            {
                _animator.SetBool("IsFalling", true);
                _animator.SetBool("IsJumping", false);
            }
            
            if (Rigid.velocity.y < 0 && _coll.IsNearGround() && !_coll.IsGround())
            {
                _animator.SetBool("IsFalling", false);
                _animator.SetTrigger("IsLandingTrigger");
                //_animator.SetBool("IsLanding", true);
            }

            if (_coll.IsGround() && !CanJump)
            {
                _animator.SetBool("IsFalling", false);
                _animator.SetBool("IsLanding", false);
                //_animator.SetBool("IsJumping", false);
            }
        }

        /// <summary>
        /// Flips the gameobject to the direction it is moving.
        /// </summary>
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
                _coyoteTimeCounter = _coyoteTime;
                _jumpLengthCounter = _jumpTime; 
                JumpCounter = _multiJumps;
            
                Wallsliding = false;
                _wallJumped = false;
                CanJump = true;
            }
            else 
            {
                // If the player is not on ground the timer decreases
                _coyoteTimeCounter -= Time.deltaTime;
                _jumpBufferCounter = JumpBufferTime;
            }
        }

        #endregion
    }
}