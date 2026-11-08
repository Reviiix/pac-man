using UnityEngine;

namespace Abstractions
{
    /// <summary>
    /// Inheriting from this class will follow the singleton pattern.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : class
    {
        public static T Instance { get; private set; }

        /// <summary>
        /// Registers this component as the instance, or removes it if another instance is already registered.
        /// </summary>
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

        /// <summary>
        /// Clears the instance only if this is the registered one, so removing a duplicate keeps the real instance.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (ReferenceEquals(Instance, this))
            {
                Instance = null;
            }
        }
    }

    /// <summary>
    /// Singleton whose instance is only accessible to the inheriting class.
    /// </summary>
    public abstract class PrivateSingleton<T> : MonoBehaviour where T : class
    {
        protected static T Instance;

        /// <summary>
        /// Registers this component as the instance, or removes it if another instance is already registered.
        /// </summary>
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

        /// <summary>
        /// Clears the instance on destroy rather than disable, because Awake will not run again to restore it.
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (ReferenceEquals(Instance, this))
            {
                Instance = null;
            }
        }
    }

    /// <summary>
    /// Shared helpers for the singleton base classes.
    /// </summary>
    internal static class SingletonUtility
    {
        /// <summary>
        /// Destroys a duplicate singleton component, using DestroyImmediate in edit mode where Destroy is not allowed.
        /// </summary>
        public static void DestroyDuplicate(Component duplicate)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(duplicate);
            }
            else
            {
                Object.DestroyImmediate(duplicate);
            }
        }
    }
}
