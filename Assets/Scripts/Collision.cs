using UnityEngine;

public class Collision : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Offset Variables")]
    [SerializeField] private Vector2 bottomOffset;
    [SerializeField] private Vector2 leftOffset;
    [SerializeField] private Vector2 rightOffset;

    [Header("Physics Material")]
    [SerializeField] private PhysicsMaterial2D plainMaterial;
    [SerializeField] private PhysicsMaterial2D stickyMaterial;
    
    // Classes
    private new Collider2D collider;

    // Variables
    private float collisionRadius = 0.25f;

    void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    #region Friction/Physics Material

    /// <summary>
    /// Changes friction/physics material for wallslide possibility
    /// </summary>
    /// <param name="isPlain"></param>
    public void FrictionChange(bool isPlain)
    {
        PhysicsMaterial2D physicsMat = isPlain ? plainMaterial : stickyMaterial;
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
        return Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        //Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, testSize, groundLayer);
    }

    public bool OnWall()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, wallLayer) || 
            Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, wallLayer);
        //Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, testSize, groundLayer);
    }

    public bool OnRightWall()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, wallLayer);
    }

    public bool OnLeftWall()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, wallLayer);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        //Physics2D.OverlapBox((Vector2)transform.position + bottomOffset, testSize, groundLayer);
    }
}
