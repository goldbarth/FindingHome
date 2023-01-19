using System;
using DataPersistence;
using UnityEngine;

namespace CollectablesBehavior
{
    public class Collectable : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private string id;
        
        public SpriteRenderer visual;
        private bool _isCollected = false;
        
        // generates an id for the item in scene
        [ContextMenu("Generate guid for id")]
        private void GenerateGuid()
        {
            id = Guid.NewGuid().ToString();
        }

        private void Update()
        {
            print(_isCollected);
        }

        public void LoadData(ref GameData data)
        {
            // if the id is not found, the item doesn´t render
            data.collectables.TryGetValue(id, out _isCollected);
            if (_isCollected)
            {
                visual.gameObject.SetActive(false);
            }
        }

        public void SaveData(ref GameData data)
        {
            if (data.collectables.ContainsKey(id)) // if the id is already in the dictionary, it is updated
            {
                data.collectables[id] = _isCollected;
            }
            else // if the id is not in the dictionary, it is added
            {
                data.collectables.Add(id, _isCollected);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _isCollected = true;
                visual.gameObject.SetActive(false);
            }
        }
    }
}
