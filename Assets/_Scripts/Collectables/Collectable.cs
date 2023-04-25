using DataPersistence;
using UnityEngine;
using System;

namespace Collectables
{
    public class Collectable : MonoBehaviour, IDataPersistence
    {
        [Header("Collectable Type")] [SerializeField]
        private CollectableType _type;

        [SerializeField] private string _id;
        [Space] [SerializeField] private AudioSource _audioSource;
        [SerializeField] private SpriteRenderer _visual;
        
        private bool _isCollected = false;

        // generates an id for the item in scene
        [ContextMenu("Generate guid for id")]
        private void GenerateGuid()
        {
            _id = Guid.NewGuid().ToString();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _audioSource.Play();
                _isCollected = true;
                _visual.gameObject.SetActive(false);
            }
        }

        public void LoadData(GameData data)
        {
            if (_type == CollectableType.Eatable)
                data.eatables.TryGetValue(_id, out _isCollected);
            else if (_type == CollectableType.Can) 
                data.collectables.TryGetValue(_id, out _isCollected);

            if (_isCollected)
                _visual.gameObject.SetActive(false);
        }

        public void SaveData(GameData data)
        {
            if (_type == CollectableType.Eatable)
            {
                // if the id is already in the dictionary, it is updated
                if (data.eatables.ContainsKey(_id))
                    data.collectables[_id] = _isCollected;
                else // if the id is not in the dictionary, it is added
                    data.eatables.Add(_id, _isCollected);
            }
            else if (_type == CollectableType.Can)
            {
                if (data.collectables.ContainsKey(_id))
                    data.collectables[_id] = _isCollected;
                else
                    data.collectables.Add(_id, _isCollected);
            }
        }
    }
}