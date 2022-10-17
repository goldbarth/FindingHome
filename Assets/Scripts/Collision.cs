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
    [SerializeField] private Vector2 offsetDown;
    [SerializeField] private Vector2 offsetUp;

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
    }

    public bool OnWall()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, wallLayer) || 
            Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, wallLayer);
    }

    public bool OnRightWall()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, wallLayer);
    }

    public bool OnLeftWall()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, wallLayer);
    }

    public bool IsNearGround(float distance)
    {
        Vector2 position = (Vector2)transform.position + offsetDown;
        Vector2 direction = Vector2.down;
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);
        return hit.collider != null;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
    }
}
