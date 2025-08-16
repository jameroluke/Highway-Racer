using UnityEngine;

public class DisableZone : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        ObstaclePoolManager poolManager = ObstaclePoolManager.Instance;
        if (poolManager != null) {
            poolManager.Despawn(other.gameObject);
        } else {
            Debug.LogWarning("ObjectPoolManager instance not found. Cannot despawn object.");
        }
    }
}
