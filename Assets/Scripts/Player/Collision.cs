using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(Collider2D), typeof(BoxCollider2D))]
    public class Collision : MonoBehaviour
    {
        [Header("Collider")] [Space]
        [SerializeField] private BoxCollider2D boxCollider; // Inspector view only to assign instance for gizmos, can be deleted if visualization is needed anymore

        [Space]
    
        [Header("Layers")] [Space]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask wallLayer;
    
        [Space]

        [Header("Physics Material")] [Space]
        [SerializeField] private PhysicsMaterial2D plainMaterial;
        [SerializeField] private PhysicsMaterial2D stickyMaterial;
    
        private Collider2D _collider;
        private Bounds Bounds => boxCollider.bounds;

        private readonly Vector3 _reduceX = new (0.03f, 0f, 0f); // Overlapbox size x -> groundcheck
        private readonly Vector2 _offsetX = new (0.01f, 0f); // Overlapbox offset -> wallcheck
        private readonly Vector2 _offsetY = new (0f, -0.01f); // Overlapbox offset -> groundcheck
        private readonly Vector2 _offset = new (0.01f, -0.01f); // Overlapbox offset -> wallcheck + groundcheck
        private float _angle; // Don´t need angles rn, but it can be useful in the future

        public bool IsGround() { return OnGround(); }
        public bool IsWall() { return OnWall(); }
        public bool IsLeftWall() { return OnLeftWall(); }
        public bool IsRightWall() { return OnRightWall(); }
    
        public bool IsNearGround() { return NearGround(); }
    
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            boxCollider = GetComponent<BoxCollider2D>();
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
        /// Changes friction/physics material on gameobject for ie. wallslide possibility.
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
            _collider.sharedMaterial = physicsMat;
            _collider.enabled = true;
        }

        #endregion

        #region Bools

        private bool OnGround()
        {
            return Physics2D.OverlapBox((Vector2)Bounds.center + _offsetY, Bounds.size - _reduceX, _angle, groundLayer);
        }

        private bool OnWall()
        {
            return Physics2D.OverlapBox((Vector2)boxCollider.bounds.center + _offsetX, Bounds.size, _angle, wallLayer) || 
                   Physics2D.OverlapBox((Vector2)boxCollider.bounds.center + (-_offsetX), Bounds.size, _angle, wallLayer);
        }

        private bool OnRightWall()
        {
            return Physics2D.OverlapBox((Vector2)boxCollider.bounds.center + _offsetX, Bounds.size, _angle, wallLayer);
        }

        private bool OnLeftWall()
        {
            return Physics2D.OverlapBox((Vector2)boxCollider.bounds.center + (-_offsetX), Bounds.size, _angle, wallLayer);
        }
    
        private bool NearGround()
        {
            return Physics2D.OverlapBox((Vector2)boxCollider.bounds.center + _offset, Bounds.size, _angle, groundLayer);
        }
    
        #endregion

#if UNITY_EDITOR
    
        private void OnDrawGizmos()
        {
            // GroundCheck
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube((Vector2)boxCollider.bounds.center + _offsetY, Bounds.size - _reduceX);
            // RightWallCheck
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube((Vector2)boxCollider.bounds.center + _offsetX, Bounds.size);
            // LeftWallCheck
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube((Vector2)boxCollider.bounds.center + -_offsetX, Bounds.size);
            // NearGround
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)boxCollider.bounds.center + _offset, Bounds.size);
        }
    
#endif
    }
}