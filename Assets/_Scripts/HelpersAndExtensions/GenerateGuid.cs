using UnityEngine;
using System;

namespace HelpersAndExtensions
{
    /// <summary>
    /// Generates a guid for the id of the item.<br/>
    /// Right click on the script in the inspector and select Generate guid for id.
    /// </summary>
    public abstract class GenerateGuid : MonoBehaviour
    {
        [Header("Item ID")]
        [SerializeField] protected internal string _id;

        [ContextMenu("Generate guid for id")]
        private void GenerateId()
        {
            _id = Guid.NewGuid().ToString();
        }
    }
}