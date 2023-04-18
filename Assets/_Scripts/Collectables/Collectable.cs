using DataPersistence;
using UnityEngine;
using System;

enum CollectableType
{
    eatable,
    collectable
}

namespace Collectables
{
    public class Collectable : MonoBehaviour, IDataPersistence
    {
        [Header("Collectable Type")] [SerializeField]
        private CollectableType type;

        [Space] [SerializeField] private AudioSource audioSource;
        [SerializeField] private string id;

        [SerializeField] private SpriteRenderer visual;
        private bool _isCollected = false;

        // generates an id for the item in scene
        [ContextMenu("Generate guid for id")]
        private void GenerateGuid()
        {
            id = Guid.NewGuid().ToString();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                audioSource.Play();
                _isCollected = true;
                visual.gameObject.SetActive(false);
            }
        }

        public void LoadData(GameData data)
        {
            if (type == CollectableType.eatable)
                data.eatables.TryGetValue(id, out _isCollected);
            else if (type == CollectableType.collectable) 
                data.collectables.TryGetValue(id, out _isCollected);

            if (_isCollected)
                visual.gameObject.SetActive(false);
        }

        public void SaveData(GameData data)
        {
            if (type == CollectableType.eatable)
            {
                // if the id is already in the dictionary, it is updated
                if (data.eatables.ContainsKey(id))
                    data.collectables[id] = _isCollected;
                else // if the id is not in the dictionary, it is added
                    data.eatables.Add(id, _isCollected);
            }
            else if (type == CollectableType.collectable)
            {
                if (data.collectables.ContainsKey(id))
                    data.collectables[id] = _isCollected;
                else
                    data.collectables.Add(id, _isCollected);
            }
        }
    }
}