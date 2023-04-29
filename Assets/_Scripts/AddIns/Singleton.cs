using UnityEngine;

namespace AddIns
{
    /// <summary>
    /// Creating an instance of the class who inherits from Singleton.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> 
    {
        public static T Instance { get; private set; }
        
        [Header("SINGLETON")]
        [SerializeField] private bool _dontDestroyOnLoad = false;
        
        protected virtual void Awake() 
        {
            if (Instance == null)
                Instance = (T)this;
            else 
            {
                Destroy(gameObject);
            }
        
            if(_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
    }
}