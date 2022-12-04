using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(BoxCollider2D))]
public class Collision : MonoBehaviour
{
    [Header("Box Collider")]
    [SerializeField] private BoxCollider2D box; // Inspector view only to assign instance for gizmos, can be deleted if visualization is needed anymore

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Physics Material")]
    [SerializeField] private PhysicsMaterial2D plainMaterial;
    [SerializeField] private PhysicsMaterial2D stickyMaterial;
    
    private new Collider2D collider;
    
    private Vector2 offsetX = new (0.01f, 0f); // Vector for overlapbox offset -> wallcheck
    private Vector2 offsetY = new (0f, -0.01f); // Vector for overlapbox offset -> groundcheck
    private float angle; // Don´t need angles rn, but it can be useful in the future

    public bool IsGround() { return OnGround(); }
    public bool IsWall() { return OnWall(); }
    public bool IsLeftWall() { return OnLeftWall(); }
    public bool IsRightWall() { return OnRightWall(); }
    
    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        box = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        OnGround();
        OnWall();
        OnLeftWall();
        OnRightWall();
    }
    
    #region Friction/Physics Material Swap

    /// <summary>
    /// Changes friction/physics material on gameobject for eg. wallslide possibility.
    /// </summary>
    /// <param name="isPlain">bool</param>
    public void FrictionChange(bool isPlain)
    {
        var physicsMat = isPlain 
            ? plainMaterial 
            : stickyMaterial;
        ApplyPhysicsMaterial(physicsMat);
    }

    private void ApplyPhysicsMaterial(PhysicsMaterial2D physicsMat)
    {
        collider.sharedMaterial = physicsMat;
        collider.enabled = true;
    }

    #endregion

    #region Bools
    
    public bool OnGround()
    {
        return Physics2D.OverlapBox((Vector2)box.bounds.center + offsetY, box.bounds.size, angle, groundLayer);
    }

    public bool OnWall()
    {
        return Physics2D.OverlapBox((Vector2)box.bounds.center + offsetX, box.bounds.size, angle, wallLayer) || 
               Physics2D.OverlapBox((Vector2)box.bounds.center + (-offsetX), box.bounds.size, angle, wallLayer);
    }

    public bool OnRightWall()
    {
        return Physics2D.OverlapBox((Vector2)box.bounds.center + offsetX, box.bounds.size, angle, wallLayer);
    }

    public bool OnLeftWall()
    {
        return Physics2D.OverlapBox((Vector2)box.bounds.center + (-offsetX), box.bounds.size, angle, wallLayer);
    }
    
    #endregion

#if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        // GroundCheck
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube((Vector2)box.bounds.center + offsetY, box.bounds.size);
        // RightWallCheck
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)box.bounds.center + offsetX, box.bounds.size);
        // LeftWallCheck
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)box.bounds.center + (-offsetX), box.bounds.size);
    }
    
#endif
}