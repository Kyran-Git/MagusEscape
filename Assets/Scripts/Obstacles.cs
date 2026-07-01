using UnityEngine;

/// <summary>
/// Attach to any obstacle prefab. Requires a Collider2D set to "Is Trigger".
/// On contact with the Player, tells the GameManager the run is over.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageToDeal = 1;
    [SerializeField] private bool destroyOnHit = true;

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

        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            bool tookDamage = player.HitPlayer(damageToDeal);

            // Crate impact sound only plays if the hit actually landed (not shielded)
            if (tookDamage)
            {
                AudioManager.Instance?.PlayHitBox();
            }

            // Only clear the asset from the map if it's meant to self-destruct
            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
