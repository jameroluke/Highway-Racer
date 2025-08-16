using System.Collections;
using Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager> {
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private bool isRunning = true;
    [SerializeField] private int score = 0;

    [Header("Speed")]
    [SerializeField] private float gameSpeed = 2f;
    [SerializeField, Min(0f)] private float maxGameSpeed = 8f;
    [SerializeField, Min(0f)] private float difficultyRamp = 0.03f;

    [Header("Lanes")]
    [SerializeField] private int laneCount = 4;
    [SerializeField] private float laneSpacing = 1.4f;
    [SerializeField] private int startLane = 2;

    [Header("Events")]
    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onGameOver;

    private Coroutine _scoreLoop;

    public bool IsRunning => isRunning;
    public int Score => score;
    public float GameSpeed => gameSpeed;
    public float DifficultyRamp => difficultyRamp;
    public int LaneCount => laneCount;
    public float LaneSpacing => laneSpacing;
    public int StartLane => startLane;

    public void StartGame() {
        score = 0;
        isRunning = true;
        UIManager.Instance.ShowHUD(true);
        UIManager.Instance.ShowGameOver(false);
        UIManager.Instance.UpdateScore(Score);
        ObstacleManager.Instance.BeginSpawning();
        onGameStart?.Invoke();
        if (_scoreLoop != null) {
            StopCoroutine(_scoreLoop);
        }
        _scoreLoop = StartCoroutine(ScoreRoutine());
    }

    public void GameOver() {
        if (!isRunning) {
            return;
        }
        isRunning = false;
        gameSpeed = 0f;
        onGameOver?.Invoke();
        ObstacleManager.Instance.StopSpawning();
        ObstacleManager.Instance.StopAllCars();
        UIManager.Instance.ShowGameOver(true);
        UIManager.Instance.ShowHUD(false);
        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlayMusic("gameover", false);
        if (_scoreLoop != null) {
            StopCoroutine(_scoreLoop);
            _scoreLoop = null;
        }
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Start() {
        SoundManager.Instance.PlayMusic("bgMusic", true);
    }

    private void Update() {
        if (isRunning) {
            if (difficultyRamp > 0f) {
                gameSpeed += difficultyRamp * Time.deltaTime;

                if (maxGameSpeed > 0f && gameSpeed > maxGameSpeed) {
                    gameSpeed = maxGameSpeed;
                }
            }
        }
        backgroundImage.uvRect = new Rect(backgroundImage.uvRect.position + new Vector2(0, gameSpeed * 0.1f * Time.deltaTime), backgroundImage.uvRect.size);
    }

    private IEnumerator ScoreRoutine() {
        var w = new WaitForSeconds(1f);
        while (IsRunning) {
            UIManager.Instance.UpdateScore(score += 1);
            yield return w;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        if (laneCount <= 0) {
            return;
        }

        float leftMostX = -((laneCount - 1) * 0.5f) * laneSpacing;

        int activeLane = Mathf.Clamp(startLane, 0, laneCount - 1);

        for (int i = 0; i < laneCount; i++) {
            float x = leftMostX + i * laneSpacing;
            Vector3 center = new Vector3(x, 0, 0);
            Vector3 size = new Vector3(1, 15, 0.01f);

            Gizmos.color = (i == activeLane) ? Color.green : Color.gray;
            Gizmos.DrawWireCube(center, size);
        }
    }
#endif
}
