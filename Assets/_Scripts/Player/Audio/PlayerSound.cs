using Dialogue;
using UnityEngine;

namespace Player.Audio
{
    public class PlayerSound : MonoBehaviour
    {
        [SerializeField] private AudioSource _stepSound;
        [SerializeField] private AudioSource _jumpSound;
        
        private CharMemoryManager _memoryManager;
        private PlayerController _player;
        private NpcManager _npcManager;
        private Collision _coll;
        
        private void Awake()
        {
            _memoryManager = FindObjectOfType<CharMemoryManager>();
            _npcManager = FindObjectOfType<NpcManager>();
            _player = GetComponent<PlayerController>();
            _coll = GetComponent<Collision>();
        }

        public void Update()
        {
            Footsteps();
            JumpSound();
        }

        private void JumpSound()
        {
            if(_memoryManager.IsInDialogue || _npcManager.IsInDialogue)
                return;
            if (_player.JumpAction.ReadValue<float>() > 0 && _coll.IsGround() && _player.CanJump)
                _jumpSound.Play();
            else if (_player.JumpAction.WasPerformedThisFrame() && _player.CanMultiJump) //doesnt work as intended
                _jumpSound.Play();
            if (_player.JumpAction.ReadValue<float>() > 0 && _coll.IsWall())
                _jumpSound.Play();
            Debug.Log("WasPerformedThisFrame: " + _player.JumpAction.WasPerformedThisFrame());
            Debug.Log("Can Multi Jump: " + _player.CanMultiJump);
        }

        private void Footsteps()
        {
            if (_player.InputX != 0 && _coll.IsGround() && !_coll.IsWall())
                _stepSound.enabled = true;
            else
                _stepSound.enabled = false;
        }
    }
}