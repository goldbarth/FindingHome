using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // TODO: Die Spielfigur kann durch das Gedrückthalten der Sprungtaste höher springen
    // TODO: Die Spielfigur kann einige Frames nachdem sie eine Plattform verlassen hat, noch springen und fällt nicht sofort nach unten. Dies wird im Fachjargon Coyote-Time genannt.
    // TODO: DONE - Die Spielfigur kann von Wänden abspringen. Diese Eigenschaft soll in der Engine an- und abwählbar gemacht werden.
    // TODO: DONE - Die Spielfigur kann an Wänden hinab rutschen. Diese Eigenschaft soll in der Engine an- und abwählbar gemacht werden.
    // TODO: Wird wenige Frames bevor die Spielfigur den Boden berührt die Sprungtaste gedrückt, soll diese Eingabe gespeichert werden und einen Sprung auslösen sobald sie den Boden berührt.

    private Rigidbody2D rb;
    private Collision coll;

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

    [Header("Stats")]
    [Range(0f, 20f)][SerializeField] private float moveSpeed;
    [Range(0f, 20f)][SerializeField] private float jumpForce;
    [Range(0f, 20f)][SerializeField] private float wallJumpLerp;
    [Range(0f, 15f)][SerializeField] private float wallSlideSpeed;

    // x(-axis) component
    private float moveInput;

    private bool wallJumped;
    private bool wallsliding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
    }

    private void Update()
    {
        Move();
        InAir();
        WallSlide();
        coll.FrictionChange(wallSlide);
        if (coll.OnGround()) wallJumped = false; // where put these?
    }

    #region InputAction

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (multiJump) // Muli Jump (can jump when in air)
            {
                Jump(Vector2.up);
            }
            else if (coll.OnGround()) // Simple Jump (can not jump when in air)
            {
                Jump(Vector2.up);
            }
            else if (coll.OnWall() && !coll.OnGround() && wallJump) // Jump off wall when contact is given
            {
                WallJump();
            }
        }
    }

    #endregion

    #region Handler

    private void Move()
    {
        if (!wallJumped)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
        else // in case of a wall jump the velocity is lerped to get a feeling of less control
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(moveInput * moveSpeed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;
    }

    private void WallJump()
    {
        Vector2 wallDir = coll.OnRightWall() ? Vector2.left : Vector2.right; // if it is the right wall the jump direction is left and reverse
        Jump(Vector2.up / 1.5f + wallDir / 1.5f);
        wallJumped = true;
    }

    private void WallSlide()
    {
        if (coll.OnWall() && !coll.OnGround() && moveInput != 0 && wallSlide)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, 0)); // TODO: variables, trial and error
            wallsliding = true;
        }
        else
        {
            wallsliding = false; 
        }
    }

    private void InAir()
    {
        if (!coll.OnGround() && !coll.OnWall())
        {
            float fallMultiplier = 2.5f;
            float lowJumpMultiplier = 2f;
            if (rb.velocity.y < 0f) // let the gameobject get more lightwheighted  at jumpstart
            {
                rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // subtracting the multiplier let the gravity multiply by 1.5
            }
            // TODO: find a good solution for contex.performed for else if
            else if (rb.velocity.y > 0f) // let the gameobject get more heavywheighted at the highest (jumping)point
            {
                rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
            }
        }
    }

    #endregion
}
