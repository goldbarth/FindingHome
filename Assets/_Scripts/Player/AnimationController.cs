using static AnimationHandler.PlayerAnimationState;
using System.Collections;
using AnimationHandler;
using Dialogue;
using UnityEngine;

namespace Player
{
  public class AnimationController : MonoBehaviour
    {
        private const float LANDING_TIME = 0.08f;
        
        private PlayerController2D _player;
        private Collision _coll;

        private bool _isClimbing;
        private bool _isLanding;
        
        private void Awake()
        {
            _coll = GetComponent<Collision>();
            _player = GetComponent<PlayerController2D>();
        }
        
        private void Update()
        {
            // keeps animation in idle when in dialogue
            if (DialogueManager.Instance.OnDialogueIsActive)
            {
                AnimationManager.Instance.SetAnimationState(player_idle);
                return;
            }

            if (_coll.IsGround() && !_player.IsRunning && !_player.JumpAction.IsPressed() 
                && !_isLanding && !_isClimbing)
                AnimationManager.Instance.SetAnimationState(_player.InputX != 0
                    ? player_walk
                    : player_idle);

            var onLandingCanceled = _coll.IsGround() && _player.JumpAction.IsPressed();
            if (onLandingCanceled)
                AnimationManager.Instance.SetAnimationState(player_idle);

            if (_coll.IsGround() && _player.IsRunning)
                AnimationManager.Instance.SetAnimationState(player_run);
            
            if (_player.IsDashing)
                AnimationManager.Instance.SetAnimationState(player_dash);

            if (_player.Rigid.velocity.y > 0 && !_isClimbing && !_coll.IsWall() && !_player.IsDashing)
            {
                AnimationManager.Instance.SetAnimationState(player_jump);
            }
            else if (_player.Rigid.velocity.y < 0 && !_coll.IsNearGround() && !_coll.IsGround() 
                     && !_player.Wallsliding && !_isClimbing && !_coll.IsWall() && !_player.IsDashing)
            {
                AnimationManager.Instance.SetAnimationState(player_fall);
            }
            
            if (_player.Rigid.velocity.y < 0 && _coll.IsNearGround() && !_player.Wallsliding) 
                _player.StartCoroutine(LandingAnimation());
                
            var isOnWall = _player.InputX != 0 && _coll.IsWall();
            if (isOnWall && _player.InputY > 0)
            {
                AnimationManager.Instance.SetAnimationState(player_wallclimb);
                _player.Wallsliding = false;
                _isClimbing = true;
            }
            else
                _isClimbing = false;
            
            if (isOnWall && _player.InputY == 0 && !_player.Wallsliding && !_isClimbing)
                AnimationManager.Instance.SetAnimationState(player_wallgrab);

            if (isOnWall && _player.Wallsliding && !_isClimbing)
                AnimationManager.Instance.SetAnimationState(player_wallslide);
        }

        private IEnumerator LandingAnimation()
        {
            _isLanding = true;
            AnimationManager.Instance.SetAnimationState(player_land);
            yield return new WaitForSeconds(LANDING_TIME);
            _isLanding = false;
        }
    }
}