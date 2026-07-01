using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Health System")]
    [SerializeField] private int maxHealth = 3;
    public int currentHealth { get; private set; }

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

    private void Start()
    {
        // Initialize player health at full on start
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Reduces player health and checks for game over condition.
    /// </summary>
    public void DamagePlayer(int damage)
    {
        if (IsGameOver) return;

        currentHealth -= damage;
        Debug.Log($"Player took damage! Current HP: {currentHealth}");

        // Hook up your heart UI animations right here later!

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        IsGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Freeze the world physics
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnGUI()
    {
        // 1. Configure the text design styling
        GUIStyle textStyle = new GUIStyle();
        textStyle.fontSize = 28;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.normal.textColor = Color.white;

        // 2. Paint the current HP status in the top left corner
        GUI.Label(new Rect(20, 20, 300, 50), "❤️ HP: " + currentHealth, textStyle);

        // 3. Paint a massive warning indicator if the player is dead
        if (IsGameOver)
        {
            GUIStyle deadStyle = new GUIStyle();
            deadStyle.fontSize = 48;
            deadStyle.fontStyle = FontStyle.Bold;
            deadStyle.normal.textColor = Color.red;

            GUI.Label(new Rect(Screen.width / 2f - 150f, Screen.height / 2f - 60f, 400, 100), "GAME OVER", deadStyle);

            textStyle.fontSize = 18;
            GUI.Label(new Rect(Screen.width / 2f - 145f, Screen.height / 2f + 10f, 400, 50), "Press 'R' or Click Restart to try again", textStyle);
        }
    }
}