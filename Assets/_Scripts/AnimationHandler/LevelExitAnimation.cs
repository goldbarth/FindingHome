using System.Collections;
using Player;
using SceneHandler;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnimationHandler
{
    public class LevelExitAnimation : MonoBehaviour
    {
        [SerializeField] private Transform exitPoint;
        [SerializeField] private float timeToWait = 2f;
        
        private Rigidbody2D _rigidBody;
        private Animator _animator;
        private GameObject _player;
        
        private readonly float _timeToBeam = 0.3f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidBody = FindObjectOfType<PlayerController2D>().GetComponent<Rigidbody2D>();
            _player = FindObjectOfType<PlayerController2D>().gameObject;
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                StartCoroutine(AnimateBeforeExit());
            }
        }

        private IEnumerator AnimateBeforeExit()
        {
            _rigidBody.transform.position = exitPoint.position;
            _rigidBody.bodyType = RigidbodyType2D.Static;
            AnimationManager.Instance.SetAnimationState(PlayerAnimationState.player_idle);
            _animator.Play("portal_warp");
            yield return new WaitForSeconds(_timeToBeam);
            _player.SetActive(false);
            _animator.Play("portal_idle");
            yield return new WaitForSeconds(timeToWait);
            Debug.LogAssertion("EXIT LEVEL 1");
            //SceneLoader.Instance.LoadSceneAsync(SceneIndex.Level2);
            //SceneLoader.Instance.UnloadSceneAsync();
        }
    }
}