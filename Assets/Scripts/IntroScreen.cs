using UnityEngine;

/// <summary>
/// Freezes the game on load behind a full-screen intro panel (showing the
/// ComicIntro sprite + a "Click to Start" prompt) until the player clicks
/// or presses a key, then unfreezes and hides the panel.
/// </summary>
public class IntroScreen : MonoBehaviour
{
    [Header("Intro Panel (Image with ComicIntro sprite + prompt text)")]
    [SerializeField] private GameObject introPanel;

    private bool hasStarted = false;

    private void Start()
    {
        Time.timeScale = 0f;
        if (introPanel != null) introPanel.SetActive(true);
    }

    private void Update()
    {
        if (hasStarted) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        hasStarted = true;
        Time.timeScale = 1f;
        if (introPanel != null) introPanel.SetActive(false);
    }
}
