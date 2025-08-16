using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;


public class ObstaclePoolManager : MonoSingleton<ObstaclePoolManager> {
    [Header("Obstacle Pool Settings")]
    [SerializeField] private GameObject[] carPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> pool;
    private List<GameObject> activeObjects = new List<GameObject>();

    public List<GameObject> GetActiveObects() {
        return activeObjects;
    }

    public GameObject Spawn(Vector3 pos, Quaternion rot) {
        GameObject go;
        if (pool.Count > 0) {
            go = pool.Dequeue();
        }
        else {
            go = Instantiate(carPrefab[Random.Range(0, carPrefab.Length)], transform);
        }

        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);

        activeObjects.Add(go);
        return go;
    }

    public void Despawn(GameObject go) {
        if (go == null) return;

        go.SetActive(false);
        go.transform.SetParent(transform);

        activeObjects.Remove(go);
        pool.Enqueue(go);
    }
    private void Start() {
        pool = new Queue<GameObject>(poolSize);
        for (int i = 0; i < poolSize; i++) {
            var go = Instantiate(carPrefab[Random.Range(0, carPrefab.Count())], transform);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }
}

