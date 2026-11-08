using UnityEngine;

namespace Abstractions
{
    /// <summary>
    /// Inheriting from this class will follow the singleton pattern.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : class
    {
        public static T Instance { get; private set; }

        protected virtual void OnEnable()
        {
            if (Instance != null && !ReferenceEquals(Instance, this))
            {
                Debug.LogError($"Duplicate singleton {typeof(T)} found on {name}. Removing the duplicate.", this);
                SingletonUtility.DestroyDuplicate(this);
                return;
            }

            Instance = this as T;
        }

        protected virtual void OnDisable()
        {
            //Only the active instance may clear itself, otherwise destroying a duplicate would wipe the real one.
            if (ReferenceEquals(Instance, this))
            {
                Instance = null;
            }
        }
    }

    public abstract class PrivateSingleton<T> : MonoBehaviour where T : class
    {
        protected static T Instance;

        protected virtual void Awake()
        {
            if (Instance != null && !ReferenceEquals(Instance, this))
            {
                Debug.LogError($"Duplicate singleton {typeof(T)} found on {name}. Removing the duplicate.", this);
                SingletonUtility.DestroyDuplicate(this);
                return;
            }

            Instance = this as T;
        }

        //Awake only runs once, so the instance is cleared on destroy rather than disable so it survives being re-enabled.
        protected virtual void OnDestroy()
        {
            if (ReferenceEquals(Instance, this))
            {
                Instance = null;
            }
        }
    }

    internal static class SingletonUtility
    {
        public static void DestroyDuplicate(Component duplicate)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(duplicate);
            }
            else
            {
                //Destroy is not allowed in edit mode (singletons using [ExecuteInEditMode]).
                Object.DestroyImmediate(duplicate);
            }
        }
    }
}
