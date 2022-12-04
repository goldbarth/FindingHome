using UnityEngine.InputSystem;
using UnityEngine;
using static Controls;
// TODO: thoughts: Movement start/stop physics

//-------------------------------------------------------------------------------------------------------------------------
// Originally, this task comes from SAE Diploma (Games Programming) and is now being further developed.
// The purpose is to develop this class as a 2D-Controller Prototype with different Features and Options
// to de-/select and use it as a base(-blueprint) for new projects/prototypes.
//-------------------------------------------------------------------------------------------------------------------------

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour, IGameplayActions
{
    #region Feature Modes

    // Features to enable/disable in unity inspector
    [Header("Multi-Jump Modes")]
    [Space]
    [Tooltip("If the Checkbox is checked Multi-Jump is on. " +
             "And you can jump (x)times when in the air.")]
    [SerializeField]
    private bool multiJump;

    [Tooltip("The amount of jumps when in air and Multi-Jump is enabled (2 or 3).")] [Range(2, 3)] [SerializeField]
    private int multiJumps;
    
    [Space]
    
    [Header("Wall-Feature Modes")]
    [Space]
    [Tooltip("If the Checkbox is checked Wall Jump is on. " +
             "If it�s unchecked it is not possible to jump off a wall.")]
    [SerializeField]
    private bool wallJump = true;

    [Tooltip("If the Checkbox is checked Wall Sliding is on. " +
             "If it�s unchecked it is not possible to slide down a wall.")]
    [SerializeField]
    private bool wallSlide = true;

    #endregion

    #region Stats
    [Space]
    // Status variables to tweak around -> get a smooth gaming experience
    [Header("Stats")]
    [Space]
    [Tooltip("The movement-speed value to in/decrease the velocity")] [Range(0f, 10f)] [SerializeField]
    private float moveSpeed;

    [Tooltip("The force value to to in/decrease the jump-height.")] [Range(0f, 10f)] [SerializeField]
    private float jumpForce;
    [Space]
    [Tooltip("The possible amount of time in air when jumping.")] [Range(0f, 0.5f)] [SerializeField]
    private float jumpTime;

    [Tooltip("The possible amount of time to jump when leaving the platform.")] [Range(0f, 0.4f)] [SerializeField]
    private float coyoteTime;
    [Space]
    [Tooltip("The lerp value to in-/decrease the 'jump-feeling' off a wall.")] [Range(0f, 4f)] [SerializeField]
    private float wallJumpLerp;

    [Tooltip("The slide speed value to in-/decrease the slide velocity at a wall.")] [Range(0f, 4f)] [SerializeField]
    private float wallSlideSpeed;
    [Space]
    [Tooltip("The gravity multiplier when the char y-velocity is > 0.")] [Range(0f, 10f)] [SerializeField]
    private float fallMultiplier = 2.5f;

    [Tooltip("The gravity multiplier when the char y-velocity is < 0.")] [Range(0f, 10f)] [SerializeField]
    private float lowJumpMultiplier = 2f;

    #endregion

    #region Fields

    // Components and classes
    private Controls controls;
    private Rigidbody2D rb;
    private Collision coll;

    // Variables
    private float moveInput; // x(-axis) component
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float jumpLengthCounter;
    private int jumpCounter;

    // Bools
    private bool isJumping;
    private bool wallJumped;
    private bool wallsliding;
    private bool canMove = true;

    #endregion

    #region Event Functions

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        controls = new Controls();
        controls.Gameplay.SetCallbacks(this);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        coll.FrictionChange(wallSlide);
        ResetterAndCounter();
    }

    private void FixedUpdate()
    {
        InAirBehavior();
        WallSlide();
        LongJump();
        Move();
    }

    #endregion

    #region Gameplay Action

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        JumpHandler(context);
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles the movement input.
    /// </summary>
    private void Move()
    {
        if (!canMove) return;

        if (!wallJumped)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
        else // In case of a wall jump the velocity is "lerped" to get a (immersive) feeling of less control.
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(moveInput * moveSpeed, rb.velocity.y)),
                wallJumpLerp * Time.deltaTime);
        }
    }

    private void JumpHandler(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            MultiJump();
            WallJump();
            isJumping = true;
        }
    }

    // (Base-)Jump without parameters
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpForce;
    }

    // (Base-)Jump with parameters
    private void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }

    /// <summary>
    /// It does multiple jumps(x) and counts down the possible jumps(x) till zero.
    /// </summary>
    private void MultiJump()
    {
        if (multiJump && jumpCounter > 0) // Multi-Jump (can jump multiple times when in air).
        {
            Debug.Log(jumpCounter);
            Jump();
            jumpCounter--;
        }
    }

    // TODO: Better Walljump
    /// <summary>
    /// It does a jump off a wall when this object has contact with a wall object and the move direction is pressed.
    /// </summary>
    private void WallJump()
    {
        if (controls.Gameplay.Move.IsPressed() && wallJump && coll.IsWall() && !coll.IsGround() ) // Jump off the wall when contact is given and the move direction is pressed.
        {
            var wallDirection =
                coll.IsRightWall()
                    ? Vector2.left
                    : Vector2.right; // If it is the right wall the jump direction is left and reverse
            Jump(Vector2.up / 1.5f + wallDirection / 1.5f);
            wallJumped = true;
        }
    }

    /// <summary>
    /// It does a higher jump when pressing the jump button and counts down a timer till zero. At the latest then the jump button is released.
    /// </summary>
    private void LongJump()
    {
        if (controls.Gameplay.Jump.WasPressedThisFrame())
            jumpBufferCounter = jumpBufferTime;
        
        if (controls.Gameplay.Jump.IsPressed())
            jumpLengthCounter -= Time.deltaTime;

        // Longer airtime when space is hold
        if (jumpLengthCounter > 0f && coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            Jump();
            isJumping = true;
            jumpBufferCounter = 0f;
        }
        else
            isJumping = false;
    }
    
    #endregion

    /// <summary>
    /// Handles the gravity in air for more immersive experience.
    /// </summary>
    private void InAirBehavior() // More immersive jump experience. Source: BetterJump (Youtube)
    {
        if (wallsliding && !(coll.OnGround() && coll.OnWall()))
        {
            switch (rb.velocity.y)
            {
                // Let the gameobject get more lightweight at jumpstart
                case < 0f:
                    rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // Subtracting the multiplier let the gravity multiply by 1.5
                    break;
                case > 0f when !controls.Gameplay.Jump.IsPressed():
                    rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // Let the gameobject get more heavyweight at the highest (jumping)point
                    break;
            }
        }
    }
    
    /// <summary>
    /// It does a down-slide at a wall when this object has contact with a wall object.
    /// </summary>
    private void WallSlide()
    {
        const float maxClampValue = 0;
        if (moveInput != 0 && wallSlide && coll.OnWall() && !coll.OnGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, maxClampValue));
            wallsliding = true;
        }
        else
            wallsliding = false;
    }

    #region Resetter / Counter

    /// <summary>
    /// Handles the variable values when certain events occur.
    /// </summary>
    private void ResetterAndCounter()
    {
        if (!controls.Gameplay.Jump.IsPressed() && coll.OnGround())
        {
            wallsliding = false;
            wallJumped = false;
            coyoteTimeCounter = coyoteTime;
            jumpLengthCounter = jumpTime;
            jumpCounter = multiJumps;
        }
        else
        {
            // If the player is not on ground the timer decreases
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Prevent double jumps
        if (controls.Gameplay.Jump.WasReleasedThisFrame())
            coyoteTimeCounter = 0f;
        
        // If the player is not jumping the timer decreases
        if (!isJumping) 
            jumpBufferCounter -= Time.deltaTime;
    }
    
    #endregion
    
}