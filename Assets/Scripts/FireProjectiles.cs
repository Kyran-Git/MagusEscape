using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FireProjectiles : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 4f;

    // Set this per-prefab in the Inspector: LeftFire prefab = (-1, 0), RightFire prefab = (1, 0)
    [SerializeField] private Vector2 direction = Vector2.left;

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

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerHitObstacle();
        }

        Destroy(gameObject);
    }
}
