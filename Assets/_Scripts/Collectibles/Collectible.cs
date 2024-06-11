using DataPersistence;
using HelpersAndExtensions;
using UnityEngine;

namespace Collectibles
{
    public class Collectible : GenerateGuid, IDataPersistence
    {
        [Header("Collectable Type")] 
        [SerializeField] private CollectableType _type;
        [Space] [SerializeField] private AudioSource _audioSource;
        [SerializeField] private SpriteRenderer _visual;
        
        private bool _isCollected = false;

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
                data.edibles.TryGetValue(_id, out _isCollected);
            else if (_type == CollectableType.Can) 
                data.collectibles.TryGetValue(_id, out _isCollected);

            if (_isCollected)
                _visual.gameObject.SetActive(false);
        }

        public void SaveData(GameData data)
        {
            if (_type == CollectableType.Eatable)
            {
                // if the id is already in the dictionary, it is updated
                if (data.edibles.ContainsKey(_id))
                    data.collectibles[_id] = _isCollected;
                else // if the id is not in the dictionary, it is added
                    data.edibles.Add(_id, _isCollected);
            }
            else if (_type == CollectableType.Can)
            {
                if (data.collectibles.ContainsKey(_id))
                    data.collectibles[_id] = _isCollected;
                else
                    data.collectibles.Add(_id, _isCollected);
            }
        }
    }
}