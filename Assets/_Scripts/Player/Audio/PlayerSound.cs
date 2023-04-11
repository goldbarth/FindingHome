using Dialogue;
using UnityEngine;

namespace Player.Audio
{
    public class PlayerSound : MonoBehaviour
    {
        [SerializeField] private AudioSource stepSound;
        [SerializeField] private AudioSource jumpSound;
        
        private CharMemoryManager _memoryManager;
        private NpcManager _npcManager;
        private PlayerController _player;
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
            if (_player.JumpAction.ReadValue<float>() > 0 && _coll.IsGround())
                jumpSound.Play();
            else if (_player.JumpAction.WasPerformedThisFrame() && _player.CanMultiJump) //doesnt work as intended
                jumpSound.Play();
            if (_player.JumpAction.ReadValue<float>() > 0 && _coll.IsWall())
                jumpSound.Play();
            Debug.Log("WasPerformedThisFrame: " + _player.JumpAction.WasPerformedThisFrame());
            Debug.Log("Can Multi Jump: " + _player.CanMultiJump);
        }

        private void Footsteps()
        {
            if (_player.InputX != 0 && _coll.IsGround() && !_coll.IsWall())
                stepSound.enabled = true;
            else
                stepSound.enabled = false;
        }
    }
}