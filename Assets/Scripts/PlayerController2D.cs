using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using static Controls; // <- TODO: There has to be a shorter way to do this
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
    [Header("Feature Modes")]
    [Tooltip("If the Checkbox is checked Multi-Jump is on. " +
             "And you can jump (x)times when in the air.")]
    [SerializeField]
    private bool multiJump;

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

    // Status variables to tweak around -> get a smooth gaming experience
    [Header("Stats")] [Tooltip("The movementspeed value to in/decrease the velocity")] [Range(0f, 10f)] [SerializeField]
    private float moveSpeed;

    [Tooltip("The force value to to in/decrease the jumpheight.")] [Range(0f, 10f)] [SerializeField]
    private float jumpForce;

    [Tooltip("The lerp value to in-/decrease the 'jumpfeeling' off a wall.")] [Range(0f, 4f)] [SerializeField]
    private float wallJumpLerp;

    [Tooltip("The slidespeed value to in-/decrease the slide velocity at a wall.")] [Range(0f, 4f)] [SerializeField]
    private float wallSlideSpeed;

    [Tooltip("The possible amount of time in air when jumping.")] [Range(0f, 0.5f)] [SerializeField]
    private float jumpTime;

    [Tooltip("The possible amount of time to jump when leaving the platform.")] [Range(0f, 0.4f)] [SerializeField]
    private float coyoteTime;

    [Tooltip("The amount of jumps when in air and Multi-Jump is enabled (2 or 3).")] [Range(2, 3)] [SerializeField]
    private int multiJumps;

    [Tooltip("The gravity multiplier when the char y-velocity is > 0.")] [Range(0f, 10f)] [SerializeField]
    private float fallMultiplier = 2.5f;

    [Tooltip("The gravity multiplier when the char y-velocity is < 0.")] [Range(0f, 10f)] [SerializeField]
    private float lowJumpMultiplier = 2f;

    #endregion

    #region Other Fields

    // Components and classes
    private Controls controls;
    private Rigidbody2D rb;
    private Collision coll;

    // Variables
    private float moveInput; // x(-axis) component
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float jumpTimeCounter;
    private int jumpCounter;

    // Bools
    private bool isJumping;
    private bool wallJumped;
    private bool wallsliding;
    private bool longJump;
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
        AirTime();
        InAir();
    }

    private void FixedUpdate()
    {
        Move();
        WallSlide();
    }

    #endregion

    #region Gameplay Action

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
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
        else // In case of a wall jump the velocity is lerped to get a (immersive) feeling of less control.
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(moveInput * moveSpeed, rb.velocity.y)),
                wallJumpLerp * Time.deltaTime);
        }
    }

    private void JumpHandler(InputAction.CallbackContext context)
    {
        jumpBufferCounter = jumpBufferTime;
        if (context.started)
        {
            isJumping = true;
            MultiJump();
            SingleJump();
            WallJump();
            FallJump();
        }
    }

    // Jump(-Base) without parameters
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpForce;
    }

    // Jump(-Base) with parameters
    private void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }

    /// <summary>
    /// It does a single jump and sets/resets values for the jump-mechanic.
    /// </summary>
    private void SingleJump()
    {
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !multiJump) // Simple Jump (can not jump when in air).
        {
            Jump();
            jumpTimeCounter = jumpTime;
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }
    }

    /// <summary>
    /// It does multiple jumps(x) and counts down the possible jumps(x) till zero.
    /// </summary>
    private void MultiJump()
    {
        if (multiJump && jumpCounter > 0) // Multi-Jump (can jump multiple times when in air).
        {
            Jump();
            jumpCounter--;
        }
    }

    /// <summary>
    /// If pressing the jump button in the air near the ground. The jump will release when go is on ground.
    /// </summary>
    private void FallJump()
    {
        if (coll.IsNearGround()) // If pressing jump button in the air near ground. The jump will release when on ground.
        {
            StartCoroutine(nameof(WaitforNextJump));
            jumpTimeCounter = jumpTime;
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }
    }

    /// <summary>
    /// It does a jump off a wall when this object has contact with a wall object and the move direction is pressed.
    /// </summary>
    private void WallJump()
    {
        if (coll.OnWall() && !coll.OnGround() && wallJump &&
            controls.Gameplay.Move
                .IsPressed()) // Jump off the wall when contact is given and the move direction is pressed.
        {
            var wallDirection =
                coll.OnRightWall()
                    ? Vector2.left
                    : Vector2.right; // If it is the right wall the jump direction is left and reverse
            Jump(Vector2.up / 1.5f + wallDirection / 1.5f);
            wallJumped = true;
        }
    }

    /// <summary>
    /// It does a down-slide at a wall when this object has contact with a wall object.
    /// </summary>
    private void WallSlide()
    {
        const float maxClampValue = 0;
        if (coll.OnWall() && !coll.OnGround() && moveInput != 0 && wallSlide)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, maxClampValue));
            wallsliding = true;
        }
        else
            wallsliding = false;
    }

    /// <summary>
    /// It does a higher jump when pressing the jump button and counts down a timer till zero. At the latest then the jump button is released.
    /// </summary>
    private void AirTime()
    {
        // Longer airtime when space is hold
        if (controls.Gameplay.Jump.IsPressed() && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                Jump();
                jumpTimeCounter -= Time.deltaTime;
            }
        }
        else
            isJumping = false;
    }

    /// <summary>
    /// Handles the gravity in air for more immersive experience.
    /// </summary>
    private void InAir() // More immersive jump experience. Source: BetterJump
    {
        if (!(coll.OnGround() && coll.OnWall() && wallsliding))
        {
            switch (rb.velocity.y)
            {
                // Let the gameobject get more lightwheighted  at jumpstart
                case < 0f:
                    rb.velocity +=
                        (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime *
                        Vector2.up; // Subtracting the multiplier let the gravity multiply by 1.5
                    break;
                case > 0f when controls.Gameplay.Jump.WasReleasedThisFrame():
                    rb.velocity +=
                        (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime *
                        Vector2.up; // Let the gameobject get more heavywheighted at the highest (jumping)point
                    break;
            }
        }
    }

    #endregion

    #region Resetter / Counter

    /// <summary>
    /// Handles the variable values when certain events occur.
    /// </summary>
    private void ResetterAndCounter()
    {
        // if (controls.Gameplay.Jump.WasReleasedThisFrame())
        //     isJumping = false;

        if (coll.OnGround())
        {
            wallsliding = false;
            wallJumped = false;
            coyoteTimeCounter = coyoteTime;
            jumpCounter = multiJumps;
        }
        else
        {
            // If the player is not on ground the timer decreases
            coyoteTimeCounter -= Time.deltaTime;
        }

        // If the player is not jumping the timer decreases
        if (!isJumping) 
            jumpBufferCounter -= Time.deltaTime;
        
    }
    
    #endregion

    #region Interfaces

    /// <summary>
    /// Wait till this gameobject is on the ground to execute a jump.
    /// </summary>
    /// <returns>Returns the "Jump" Method</returns>
    IEnumerator WaitforNextJump()
    {
        while (!coll.OnGround()) yield return null;
        yield return StartCoroutine(nameof(Jump));
    }

    #endregion
}