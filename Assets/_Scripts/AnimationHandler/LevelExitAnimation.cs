using System.Collections;
using Player;
using SceneHandler;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnimationHandler
{
    public class LevelExitAnimation : MonoBehaviour
    {
        public delegate void OnLevelExit();
        public static event OnLevelExit OnLevelExitEvent;
        
        [SerializeField] private Transform exitPoint;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float timeToWait = 2f;

        private Rigidbody2D _rigidBody;
        private Animator _animator;
        private Animator _playerAnimator;
        private GameObject _player;

        private const float PREPARE_TO_BEAM = 2.5f;
        private const float TIME_TO_BEAM = 1f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidBody = FindObjectOfType<PlayerController>().GetComponent<Rigidbody2D>();
            _player = FindObjectOfType<PlayerController>().gameObject;
            _playerAnimator = _player.GetComponentInChildren<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D col)
            {
                if (col.CompareTag("Player") && !col.isTrigger)
                    StartCoroutine(PortalWarp());
            }

            // NOTE: OMEGALUL
            private IEnumerator PortalWarp()
            {
                _rigidBody.transform.position = exitPoint.position;
                _rigidBody.velocity = Vector2.zero;
                _playerAnimator.SetBool("IsWalking", false);
                _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
                yield return new WaitForSeconds(PREPARE_TO_BEAM);
                _playerAnimator.Play("player_teleport");
                audioSource.Play();
                _animator.Play("portal_warp");
                yield return new WaitForSeconds(TIME_TO_BEAM);
                _player.SetActive(false);
                _animator.Play("portal_idle");
                yield return new WaitForSeconds(timeToWait);
                OnLevelExitEvent?.Invoke();
            }
        }
    }