using UnityEngine;

/// <summary>
/// Attach to any obstacle prefab. Requires a Collider2D set to "Is Trigger".
/// On contact with the Player, tells the GameManager the run is over.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{
    private void Reset()
    {
        // Default the collider to a trigger so designers don't forget to set it.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerHitObstacle();
        }
        else
        {
            Debug.LogWarning("Obstacle hit Player but no GameManager was found in the scene.");
        }
    }
}
