using UnityEngine;

namespace Common {
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {

        public static T Instance { get; private set; }
        public static bool IsInstantiated { get => Instance != null; }

        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("Multiple instances of " + typeof(T).ToString());
            }

            Instance = this as T;
            Instance.Init();
        }

        protected virtual void Init() { }
    }
}
