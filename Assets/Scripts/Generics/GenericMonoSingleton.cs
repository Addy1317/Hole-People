using UnityEngine;

namespace SlowpokeStudio.Generic
{
    public class GenericMonoSingleton<T> : MonoBehaviour where T : GenericMonoSingleton<T>
    {
        private static T instance;
        public static T Instance { get { return instance; } }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = (T)this;
                DontDestroyOnLoad(gameObject);
                Debug.Log($"{typeof(T)} instance created.");
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning($"{typeof(T)} already exists. Destroying duplicate on {gameObject.name}");
            }
        }
    }
}

