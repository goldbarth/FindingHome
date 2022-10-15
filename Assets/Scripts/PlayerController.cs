using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //private Controls controls;
    private Rigidbody2D rb;
    private Collision coll;

    
    [Header("Jump Mode")] 
    [Tooltip("If the Checkbox is checked Multi-Jump is on. " +
        "If it´s unchecked it is only possible to jump when the player is on the ground.")]
    [SerializeField] private bool multiJump;
    
    [Header("Stats")]
    [Range(0f, 25f)][SerializeField] private float moveSpeed;
    [Range(0f, 25f)][SerializeField] private float jumpForce;

    private float moveInput;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();

    }

    //public void OnEnable()
    //{
    //    controls.Player.Jump.performed += HandleJump;
    //    controls.Player.Jump.Enable();
    //}
    //public void OnDisable()
    //{
    //    controls.Player.Jump.performed -= HandleJump;
    //    controls.Player.Jump.Disable();
    //}

    private void Update()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        moveInput = context.ReadValue<Vector2>().x;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        float fallMultiplier = 2.5f;
        float lowJumpMultiplier = 2f;

        if (multiJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += Vector2.up * jumpForce;
            if (rb.velocity.y < 0f) // let the gameobject get more light at jumpstart
            {
                rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // subtracting the multiplier let the gravity multiply by 1.5
            }
            else if (rb.velocity.y > 0f && !context.performed) // let the gameobject get more heavy at the highest (jumping)point
            {
                rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
            }
        }
        else if (coll.onGround)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity += Vector2.up * jumpForce;
            if (rb.velocity.y < 0f)
            {
                rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up; // subtracting the multiplier let the gravity multiply by 1.5
            }
            else if (rb.velocity.y > 0f && !context.performed)
            {
                rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
            }
        }
    }
}
