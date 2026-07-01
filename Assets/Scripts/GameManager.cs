using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Called by Obstacle.cs when the player touches an obstacle.
    /// </summary>
    public void PlayerHitObstacle()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Freezing timeScale is the simplest way to stop the background,
        // obstacle spawning, and player physics all at once.
        // If you later want a death animation to play WHILE the world freezes,
        // swap this for per-object pausing instead (happy to add that next).
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
