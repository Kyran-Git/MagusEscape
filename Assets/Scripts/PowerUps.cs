using UnityEngine;

/// <summary>
/// Attach to any power-up pickup prefab (Book, Shield, Boost sprites).
/// Requires a Collider2D set to "Is Trigger". Set the matching
/// Power Up Type in the Inspector for each prefab.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PowerUps : MonoBehaviour
{
    [Header("Power-Up Settings")]
    [SerializeField] private PowerUpType powerUpType;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CollectPowerUp(powerUpType);
        }

        Destroy(gameObject);
    }
}