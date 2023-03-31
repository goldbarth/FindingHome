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
        [SerializeField] private float timeToWait = 2f;

        [SerializeField] private AudioSource audioSource;

        private Rigidbody2D _rigidBody;
        private Animator _animator;
        private Animator _playerAnimator;
        private GameObject _player;

        private readonly float _prepareToBeam = 2.5f;
        private readonly float _timeToBeam = 1f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidBody = FindObjectOfType<PlayerController2D>().GetComponent<Rigidbody2D>();
            _player = FindObjectOfType<PlayerController2D>().gameObject;
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
                _playerAnimator.Play("player_idle");
                _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
                yield return new WaitForSeconds(_prepareToBeam);
                _playerAnimator.Play("player_teleport");
                audioSource.Play();
                _animator.Play("portal_warp");
                yield return new WaitForSeconds(_timeToBeam);
                _player.SetActive(false);
                _animator.Play("portal_idle");
                yield return new WaitForSeconds(timeToWait);
                OnLevelExitEvent?.Invoke();
            }
        }
    }