using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpikeObstacle : MonoBehaviour
{
    [Header("Spike Attributes")]
    [SerializeField] private int damageToDeal = 1;
    [Range(0f, 1f)]
    [SerializeField] private float speedReductionPercent = 0.5f; // Slices speed in half on impact

    private void Reset()
    {
        // Spikes should always be triggers so the player passes over them while taking damage
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // 1. Inflict damage and trigger invincibility frames
            player.HitPlayer(damageToDeal);

            // 2. Inflict the physical momentum penalty
            player.ApplySpeedPenalty(speedReductionPercent);
        }
    }
}