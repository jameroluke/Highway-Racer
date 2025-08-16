using Common;
using TMPro;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager> {
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI scoreText;

    public void ShowMainMenu(bool show) {
        if (mainMenuPanel != null) {
            mainMenuPanel.SetActive(show);
        }
    }

    public void ShowHUD(bool show) {
        if (hudPanel != null) {
            hudPanel.SetActive(show);
        }
    }

    public void ShowGameOver(bool show) {
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(show);
        }
    }

    public void UpdateScore(int score) {
        if (scoreText != null) {
            scoreText.text = score.ToString();
        }
    }
}
