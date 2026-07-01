using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FireProjectiles : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private Vector2 direction = Vector2.left;

    [Header("Damage")]
    [SerializeField] private int damageToDeal = 1; // Explicitly added damage parameter

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.HitPlayer(damageToDeal);
        }

        Destroy(gameObject);
    }
}