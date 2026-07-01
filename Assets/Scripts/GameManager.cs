using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Health System")]
    [SerializeField] private int maxHealth = 3;
    public int currentHealth { get; private set; }

    [Header("Power-Up Settings")]
    [SerializeField] private int shieldOrbCapacity = 3;
    [SerializeField] private float boostDuration = 3f;
    [SerializeField] private float boostSpeedMultiplier = 1.2f;

    [Header("Power-Up Icons (drag matching sprites here)")]
    [SerializeField] private Sprite bookShieldIcon;
    [SerializeField] private Sprite shieldOrbIcon;
    [SerializeField] private Sprite boostIcon;

    [Header("Score System")]
    [SerializeField] private float scorePerDistanceUnit = 1f;
    [SerializeField] private float baseMultiplier = 1.5f;
    [SerializeField] private float maxMultiplier = 3f;
    [SerializeField] private float distanceForMaxMultiplier = 200f; // distance needed to ramp from base to max

    public int Score { get; private set; }
    public float DistanceTraveled { get; private set; }
    public float CurrentMultiplier { get; private set; }

    private float distanceSinceLastHit = 0f;
    private float scoreAccumulator = 0f; // holds fractional score between frames

    private float survivalTimer = 0f;

    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    private bool hasBookShield = false;
    private int shieldOrbRemaining = 0;
    private bool isBoostActive = false;
    private float boostTimer = 0f;

    private PlayerController player;

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
        currentHealth = maxHealth;
        player = FindObjectOfType<PlayerController>();
        CurrentMultiplier = baseMultiplier;
    }

    private void Update()
    {
        HandleRestartInput();
        HandlePauseInput();
        TickBoostTimer();
        TickScore();
    }

    private void HandleRestartInput()
    {
        if (IsGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    private void HandlePauseInput()
    {
        if (IsGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    private void TickBoostTimer()
    {
        if (!isBoostActive) return;

        // Time.deltaTime is 0 while paused/game over, so this naturally freezes too.
        boostTimer -= Time.deltaTime;
        if (boostTimer <= 0f)
        {
            isBoostActive = false;
            if (player != null) player.SetSpeedBoost(1f, false);
        }
    }

    // ---------------- Score ----------------

    private void TickScore()
    {
        if (IsGameOver) return;

        float speed = WorldManager.Instance != null ? WorldManager.Instance.currentWorldSpeed : 0f;
        float distanceThisFrame = speed * Time.deltaTime;
        DistanceTraveled += distanceThisFrame;
        distanceSinceLastHit += distanceThisFrame;

        // Multiplier ramps from baseMultiplier up to maxMultiplier as distance
        // since the last hit increases, capping out at distanceForMaxMultiplier.
        float t = Mathf.Clamp01(distanceSinceLastHit / distanceForMaxMultiplier);
        CurrentMultiplier = Mathf.Lerp(baseMultiplier, maxMultiplier, t);

        // Accumulate fractional score and only commit whole points to Score,
        // so small per-frame contributions aren't lost to rounding.
        scoreAccumulator += distanceThisFrame * scorePerDistanceUnit * CurrentMultiplier;
        int wholePoints = Mathf.FloorToInt(scoreAccumulator);
        if (wholePoints > 0)
        {
            Score += wholePoints;
            scoreAccumulator -= wholePoints;
        }
    }

    private void ResetScoreStreak()
    {
        distanceSinceLastHit = 0f;
        CurrentMultiplier = baseMultiplier;
    }

    // ---------------- Pause Menu ----------------

    public void TogglePause()
    {
        if (IsPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (IsGameOver) return;

        IsPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    // ---------------- Power-Ups ----------------

    public void CollectPowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.BookShield:
                hasBookShield = true;
                break;

            case PowerUpType.ShieldOrb:
                shieldOrbRemaining = shieldOrbCapacity;
                break;

            case PowerUpType.Boost:
                StartBoost();
                break;
        }
    }

    private void StartBoost()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();

        isBoostActive = true;
        boostTimer = boostDuration;

        if (player != null) player.SetSpeedBoost(boostSpeedMultiplier, true);
    }

    // ---------------- Damage / Health ----------------

    /// <summary>
    /// Reduces player health after accounting for any active shields.
    /// Returns true if real damage got through (used to decide whether
    /// to play hit reactions like invincibility blink or speed penalties).
    /// </summary>
    public bool DamagePlayer(int damage)
    {
        if (IsGameOver) return false;

        // Book shield: blocks this hit completely, one-time use.
        if (hasBookShield)
        {
            hasBookShield = false;
            Debug.Log("Book shield absorbed the hit completely!");
            return false;
        }

        // Shield orb: absorbs damage out of its remaining pool.
        if (shieldOrbRemaining > 0)
        {
            int absorbed = Mathf.Min(shieldOrbRemaining, damage);
            shieldOrbRemaining -= absorbed;
            damage -= absorbed;

            if (damage <= 0)
            {
                Debug.Log("Shield orb absorbed the hit! Remaining shield: " + shieldOrbRemaining);
                return false;
            }
        }

        currentHealth -= damage;
        Debug.Log($"Player took damage! Current HP: {currentHealth}");

        ResetScoreStreak();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            TriggerGameOver();
        }

        return true;
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

        // 2b. Paint score/distance/multiplier in the top right corner
        GUIStyle scoreStyle = new GUIStyle(textStyle);
        scoreStyle.fontSize = 24;
        scoreStyle.alignment = TextAnchor.UpperRight;

        string multiplierText = CurrentMultiplier > 1f ? $"  (x{CurrentMultiplier:0.00})" : "";
        string scoreText = $"Score: {Score}{multiplierText}\nDistance: {DistanceTraveled:0} m";
        GUI.Label(new Rect(Screen.width - 340, 20, 320, 60), scoreText, scoreStyle);

        // 3. Paint active power-up icons just below the HP text
        float iconSize = 40f;
        float iconX = 20f;
        float iconY = 65f;

        if (hasBookShield && bookShieldIcon != null)
        {
            GUI.DrawTexture(new Rect(iconX, iconY, iconSize, iconSize), bookShieldIcon.texture);
            iconX += iconSize + 10f;
        }

        if (shieldOrbRemaining > 0 && shieldOrbIcon != null)
        {
            GUI.DrawTexture(new Rect(iconX, iconY, iconSize, iconSize), shieldOrbIcon.texture);

            GUIStyle countStyle = new GUIStyle(textStyle);
            countStyle.fontSize = 16;
            GUI.Label(new Rect(iconX, iconY + iconSize - 8f, iconSize, 20f), shieldOrbRemaining.ToString(), countStyle);

            iconX += iconSize + 10f;
        }

        if (isBoostActive && boostIcon != null)
        {
            GUI.DrawTexture(new Rect(iconX, iconY, iconSize, iconSize), boostIcon.texture);
            iconX += iconSize + 10f;
        }

        // 4. Paint a massive warning indicator if the player is dead
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
