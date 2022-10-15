using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layer")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Bools")]
    public bool onGround;

    [Header("Variables")]
    [SerializeField] private Vector2 bottomOffset;

    private float collisionRadius = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, testSize, groundLayer);
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, testSize, groundLayer);
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
    }

    public bool IsGround()
    {
        var circle =  Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        return circle != null;
    }
}
