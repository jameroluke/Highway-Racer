using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class ObstacleManager : MonoSingleton<ObstacleManager> {
    [Header("Spawn Height")]
    [SerializeField] private float laneY = 10f;

    [Header(" Difficulty")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float minInterval = 0.25f;

    [Header("Safety Rules")]
    [SerializeField] private int maxBlockedLanesPerWave = 3;
    [SerializeField] private float minWaveGapY = 6f;
    [SerializeField] private float perLaneCooldown = 0.8f;
    [SerializeField] private float firstSpawnDelay = 0.75f;
    [SerializeField] private int graceWaves = 2;
    [SerializeField] private int minFreeLanesAtStart = 2;

    private Coroutine _spawnLoop;
    private int _laneCount;
    private float _laneSpacing;
    private float _difficultyRamp;

    private float[] _laneReadyAt;
    private int _lastFreeLane = -1;
    private int _waveIndex = 0;

    public void BeginSpawning() {
        if (_spawnLoop != null) {
            StopCoroutine(_spawnLoop);
        }
        _spawnLoop = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning() {
        if (_spawnLoop != null) {
            StopCoroutine(_spawnLoop);
            _spawnLoop = null;
        }
    }

    public void StopAllCars() {
        var pool = ObstaclePoolManager.Instance.GetActiveObects();
        foreach (var go in pool) {
            if (go.TryGetComponent<Obstacle>(out var obstacle)) {
                obstacle.enabled = false;
            }
        }
    }

    private void Start() {
        var gameManager = GameManager.Instance;
        if (gameManager == null) {
            Debug.LogError("GameManager instance not found. Please ensure GameManager is initialized before ObstacleManager.");
            return;
        }

        _laneCount = gameManager.LaneCount;
        _laneSpacing = gameManager.LaneSpacing;
        _difficultyRamp = gameManager.DifficultyRamp;

        if (maxBlockedLanesPerWave > _laneCount - 1) {
            maxBlockedLanesPerWave = Mathf.Max(1, _laneCount - 1);
        }

        _laneReadyAt = new float[_laneCount];
    }

    private IEnumerator SpawnRoutine() {
        float t = 0f;
        float interval = spawnInterval;
        var w = new WaitForEndOfFrame();

        if (firstSpawnDelay > 0f) {
            float d = 0f;
            while (d < firstSpawnDelay && GameManager.Instance != null && GameManager.Instance.IsRunning) {
                d += Time.deltaTime;
                yield return w;
            }
        }

        while (GameManager.Instance != null && GameManager.Instance.IsRunning) {
            float obstacleSpeed = GameManager.Instance.GameSpeed;

            float minGapInterval = minWaveGapY / Mathf.Max(0.01f, obstacleSpeed);

            float safeInterval = Mathf.Max(minInterval, minGapInterval);
            interval = Mathf.Max(safeInterval, spawnInterval - t * _difficultyRamp);

            SpawnWave(obstacleSpeed);
            _waveIndex++;

            float timer = 0f;
            while (timer < interval && GameManager.Instance != null && GameManager.Instance.IsRunning) {
                timer += Time.deltaTime;
                yield return w;
            }

            t += interval;
        }
    }

    private void SpawnWave(float obstacleSpeed) {
        if (_laneCount <= 0) {
            return;
        }

        int desiredFreeLanes = 1;
        bool inGrace = _waveIndex < graceWaves;
        if (inGrace) {
            desiredFreeLanes = Mathf.Clamp(minFreeLanesAtStart, 1, _laneCount);
        }


        int freeLane;
        freeLane = Random.Range(0, _laneCount);
        if (_laneCount > 2 && _lastFreeLane == freeLane) {
            int alt = Random.Range(0, _laneCount - 1);
            freeLane = alt >= freeLane ? alt + 1 : alt;
        }

        _lastFreeLane = freeLane;

        bool[] laneIsForcedFree = new bool[_laneCount];
        laneIsForcedFree[freeLane] = true;

        if (inGrace && desiredFreeLanes > 1 && _laneCount >= 2) {
            int neighbor = (freeLane < _laneCount - 1) ? freeLane + 1 : freeLane - 1;
            if (neighbor >= 0 && neighbor < _laneCount) {
                laneIsForcedFree[neighbor] = true;
            }
        }

        List<int> candidates = new List<int>(_laneCount);
        for (int i = 0; i < _laneCount; i++) {
            if (!laneIsForcedFree[i]) {
                candidates.Add(i);
            }
        }

        for (int i = 0; i < candidates.Count; i++) {
            int j = Random.Range(i, candidates.Count);
            int tmp = candidates[i];
            candidates[i] = candidates[j];
            candidates[j] = tmp;
        }

        int maxBlockable = Mathf.Min(
            maxBlockedLanesPerWave,
            candidates.Count,
            Mathf.Max(0, _laneCount - desiredFreeLanes)
        );

        int toBlock = (maxBlockable > 0) ? Random.Range(1, maxBlockable + 1) : 0;

        float leftMostX = -((_laneCount - 1) * 0.5f) * _laneSpacing;
        int spawned = 0;

        for (int idx = 0; idx < candidates.Count && spawned < toBlock; idx++) {
            int laneIndex = candidates[idx];

            if (Time.time < _laneReadyAt[laneIndex]) {
                continue;
            }

            float x = leftMostX + laneIndex * _laneSpacing;
            Vector3 pos = new Vector3(x, laneY, 0f);

            var go = ObstaclePoolManager.Instance.Spawn(pos, Quaternion.identity);
            var obs = go.GetComponent<Obstacle>();
            if (obs != null) {
                obs.Init(Random.Range(obstacleSpeed, obstacleSpeed + 0.3f));
            }

            _laneReadyAt[laneIndex] = Time.time + perLaneCooldown;
            spawned++;
        }
    }

}
