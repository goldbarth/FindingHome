using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // TODO: WIP - Die Spielfigur kann durch das Gedrückthalten der Sprungtaste höher springen

    // Features to enable/disable in unity inspector
    [Header("Feature Modes")]
    [Tooltip("If the Checkbox is checked Multi-Jump is on. " +
        "If it´s unchecked it is only possible to jump when the player is on the ground.")]
    [SerializeField] private bool multiJump = false;
    [Tooltip("If the Checkbox is checked Wall Jump is on. " +
        "If it´s unchecked it is not possible to jump off a wall.")]
    [SerializeField] private bool wallJump = true;
    [Tooltip("If the Checkbox is checked Wall Sliding is on. " +
        "If it´s unchecked it is not possible to slide down a wall.")]
    [SerializeField] private bool wallSlide = true;

    // Status variables to tweak around -> get a smooth play experience
    [Header("Stats")]
    [Tooltip("The movementspeed value to in/decrease the velocity")]
    [Range(0f, 10f)][SerializeField] private float moveSpeed;
    [Tooltip("The force value to to in/decrease the jumpheight.")]
    [Range(0f, 15f)][SerializeField] private float jumpForce;
    [Tooltip("The lerp value to in-/decrease the 'jumpfeeling' off a wall.")]
    [Range(0f, 4f)][SerializeField] private float wallJumpLerp;
    [Tooltip("The slidespeed value to in-/decrease the slide velocity at a wall.")]
    [Range(0f, 4f)][SerializeField] private float wallSlideSpeed;
    [Tooltip("The possible amount of time in air when jumping.")]
    [Range(0f, 1f)][SerializeField] private float jumpTime;
    [Tooltip("The possible amount of time to jump when leaving the platform.")]
    [Range(0f, 0.5f)][SerializeField] private float coyoteTime;
    [Tooltip("The amount of extra jumps when in air (up to 3).")]
    [Range(1, 3)][SerializeField] private int extraJump;

    // Components and classes
    private Rigidbody2D rb;
    private Collision coll;

    // Variables
    private float moveInput; // x(-axis) component
    private float jumpTimeCounter;
    
    // Coyote-Jump
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    private int extraJumpCounter;

    // Bools
    private bool isJumping;
    private bool wallJumped;
    private bool wallsliding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
    }

    private void Update()
    {
        coll.FrictionChange(wallSlide);

        #region Resetter / Counter
        
        // If the player is not jumping the timer decreases
        jumpBufferCounter -= Time.deltaTime;

        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            isJumping = false;

        if (coll.OnGround())
        {
            wallsliding = false;
            wallJumped = false;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            // If the player is not on ground the timer decreases
            coyoteTimeCounter -= Time.deltaTime;
        }

        #endregion
    }

    private void FixedUpdate()
    {
        Move();
        InAir();
        WallSlide();
    }

    #region InputAction

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            jumpBufferCounter = jumpBufferTime;
            if (multiJump) // Muli Jump (can jump when in air)
            {
                if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
                    extraJumpCounter = extraJump;
                if (extraJumpCounter > 0)
                {
                    Jump();
                    extraJumpCounter--;
                }
                else
                {
                    coyoteTimeCounter = 0f;
                    jumpBufferCounter = 0f;
                }
            }
            if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f) // Simple Jump (can not jump when in air)
            {
                Jump();
                jumpTimeCounter = jumpTime;
                coyoteTimeCounter = 0f;
                jumpBufferCounter = 0f;
            }
            if (coll.OnWall() && !coll.OnGround() && wallJump) // Jump off wall when contact is given
            {
                WallJump();
            }
        }
        // TODO: only gets triggered if the go is near ground
        if (context.started && !coll.OnGround()) // If pressing jump button in the air. The jump will release when on ground
        {
            StartCoroutine(nameof(WaitforGround));
        }
        // Longer airtime when space is pressing
        // TODO: doesn´t work as expected 
        if (Keyboard.current.spaceKey.isPressed && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                Debug.Log("JumpHiger");
                Jump(Vector2.up);
                jumpTimeCounter -= Time.deltaTime; 
            }
        }
        else
            isJumping = false;
    }

    #endregion

    #region Handler

    private void Move()
    {
        if (!wallJumped)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
        else // In case of a wall jump the velocity is lerped to get a feeling of less control
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(moveInput * moveSpeed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * jumpForce;
        isJumping = true;
    }

    private void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
        isJumping = true;
    }

    private void WallJump()
    {
        Vector2 wallDir = coll.OnRightWall() ? Vector2.left : Vector2.right; // If it is the right wall the jump direction is left and reverse
        Jump(Vector2.up / 1.5f + wallDir / 1.5f);
        wallJumped = true;
    }

    private void WallSlide()
    {
        float maxClampValue = 0;
        if (coll.OnWall() && !coll.OnGround() && moveInput != 0 && wallSlide)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, maxClampValue));
            wallsliding = true;
        }
        else
            wallsliding = false;
    }

    private void InAir()
    {
        if (!coll.OnGround() && !coll.OnWall() && wallsliding == false)
        {
            float fallMultiplier = 2.5f;
            float lowJumpMultiplier = 2f;
            if (rb.velocity.y < 0f) // Let the gameobject get more lightwheighted  at jumpstart
            {
                rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // Subtracting the multiplier let the gravity multiply by 1.5
            }
            else if (rb.velocity.y > 0f && Keyboard.current.spaceKey.wasReleasedThisFrame) // Let the gameobject get more heavywheighted at the highest (jumping)point
            {
                rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
            }
        }
    }

    #endregion

    // Wait to jump till player is on the ground again
    IEnumerator WaitforGround()
    {
        while (!coll.OnGround())
        {
            yield return null;
        }
        yield return StartCoroutine(nameof(Jump));
    }
}
