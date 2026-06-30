using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;

    [Header("Global Speed Settings")]
    public float currentWorldSpeed = 5f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float speedIncreaseRate = 0.05f; // Dynamic Difficulty

    void Awake()
    {
        // Singleton pattern so other scripts can easily check the current speed
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Gradually speed up the game over time to increase difficulty
        if (currentWorldSpeed < maxSpeed)
        {
            currentWorldSpeed += speedIncreaseRate * Time.deltaTime;
        }
    }
}