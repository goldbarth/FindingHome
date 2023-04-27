using System;
using System.Collections;
using DataPersistence;
using UnityEngine;

namespace ObstacleHandler
{
    public class FallingObstacle : MonoBehaviour
    {
        [Header("RAYCAST DISTANCE")]
        [SerializeField]private float _distance;
        
        [Header("RAYCAST ORIGIN")]
        [SerializeField]private Transform _raycastOrigin;
        
        [Header("SPRITE CHANGE WHEN HIT GROUND")]
        [SerializeField] private Sprite _newSprite;
        
        private SpriteRenderer _spriteRenderer;
        private PolygonCollider2D _coll;
        private Animator _animator;
        private Rigidbody2D _rb;
        private bool _isFalling;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _coll = GetComponent<PolygonCollider2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            Physics2D.queriesStartInColliders = false;
            if (!_isFalling)
            {
                var hit = Physics2D.Raycast(transform.position, Vector2.down, _distance);
                Debug.DrawRay(_raycastOrigin.position, Vector2.down * _distance, Color.red);
                
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    _isFalling = true;
                    StartCoroutine(AnimateBeforeFalling());
                }
            }
        }

        private IEnumerator AnimateBeforeFalling()
        {
            _animator.Play("sword_falling"); 
            yield return new WaitForSeconds(.3f);
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }
            

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                DataPersistenceManager.Instance.LoadGame();
            }
            else if (col.gameObject.CompareTag("Ground"))
            {
                _coll.enabled = false;
                _rb.bodyType = RigidbodyType2D.Static;
                _spriteRenderer.sprite = _newSprite;
                _animator.enabled = false;
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_raycastOrigin.position, Vector2.down * _distance);
        }
#endif
    }
}