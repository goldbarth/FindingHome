using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [RequireComponent(typeof(Collider2D))]
    public class Collision : MonoBehaviour
    {
        [Header("LAYERS")] [Space]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _wallLayer;
        
        [Space] [Header("PHYSICS MATERIAL")] [Space]
        [SerializeField] private PhysicsMaterial2D _plainMaterial;
        [SerializeField] private PhysicsMaterial2D _stickyMaterial;
        
        [Space] [Header("COLLIDER")]
        [SerializeField] private PolygonCollider2D _polygonCollider;

        private readonly Vector3 _reduceSize = new (0.25f, 1.45f, 0f); // Overlapbox size x -> groundcheck
        private readonly Vector2 _offsetX = new (0.01f, 0f); // Overlapbox offset -> wallcheck
        private readonly Vector2 _offsetY = new (0f, -0.745f); // Overlapbox offset -> groundcheck
        private readonly Vector2 _offset = new (0.01f, -0.35f); // Overlapbox offset -> wallcheck + groundcheck
        
        private Collider2D _collider;
        private float _angle;
        
        private Bounds Bounds => _polygonCollider.bounds;

        public bool IsGround() { return OnGround(); }
        public bool IsWall() { return OnWall(); }
        public bool IsLeftWall() { return OnLeftWall(); }
        public bool IsRightWall() { return OnRightWall(); }
    
        public bool IsNearGround() { return NearGround(); }
    
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
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
                ? _plainMaterial 
                : _stickyMaterial;
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
            return Physics2D.OverlapBox((Vector2)Bounds.center + _offsetY, Bounds.size - _reduceSize, _angle, _groundLayer);
        }

        private bool OnWall()
        {
            return Physics2D.OverlapBox((Vector2)Bounds.center + _offsetX, Bounds.size, _angle, _wallLayer) || 
                   Physics2D.OverlapBox((Vector2)Bounds.center + (-_offsetX), Bounds.size, _angle, _wallLayer);
        }

        private bool OnRightWall()
        {
            return Physics2D.OverlapBox((Vector2)Bounds.center + _offsetX, Bounds.size, _angle, _wallLayer);
        }

        private bool OnLeftWall()
        {
            return Physics2D.OverlapBox((Vector2)Bounds.center + (-_offsetX), Bounds.size, _angle, _wallLayer);
        }
    
        private bool NearGround()
        {
            return Physics2D.OverlapBox((Vector2)Bounds.center + _offset, Bounds.size, _angle, _groundLayer);
        }
    
        #endregion

#if UNITY_EDITOR
    
        private void OnDrawGizmos()
        {
            // GroundCheck
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube((Vector2)Bounds.center + _offsetY, Bounds.size - _reduceSize);
            // RightWallCheck
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube((Vector2)Bounds.center + _offsetX, Bounds.size);
            // LeftWallCheck
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube((Vector2)Bounds.center + -_offsetX, Bounds.size);
            // NearGround
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)Bounds.center + _offset, Bounds.size);
        }
    
#endif
    }
}