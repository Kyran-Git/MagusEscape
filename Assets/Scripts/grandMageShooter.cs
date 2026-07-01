using UnityEngine;

public class grandMageShooter : MonoBehaviour
{
    [Header("Fire Prefab")]
    [SerializeField] private GameObject firePrefab;

    [Header("Fire Point")]
    [SerializeField] private Transform firePoint;

    [Header("Timing")]
    [SerializeField] private float fireInterval = 0.5f;

    [Header("Room Boundary")]
    [Tooltip("Assign this room's 'floor' Collider2D. Firing stops once this collider's left edge reaches the player.")]
    [SerializeField] private Collider2D roomFloor;

    private Transform player;
    private float fireTimer;
    private bool isFiring = true;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        fireTimer = fireInterval;
    }

    private void Update()
    {
        if (!isFiring || player == null) return;

        // Stop firing once the room's left edge has reached the player's position —
        // that's the moment the player is effectively leaving this room.
        if (roomFloor != null && roomFloor.bounds.min.x <= player.position.x)
        {
            isFiring = false;
            return;
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireAtPlayer();
            fireTimer = fireInterval;
        }
    }

    private void FireAtPlayer()
    {
        if (firePrefab == null || firePoint == null) return;

        Instantiate(firePrefab, firePoint.position, firePoint.rotation);
        AudioManager.Instance?.PlayFireball();
    }
}
