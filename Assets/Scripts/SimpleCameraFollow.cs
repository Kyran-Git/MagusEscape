using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // Drag your Player object here
    [SerializeField] private float aheadOffset = 3f;     // Pushes camera slightly ahead

    private float fixedY;
    private float fixedZ;

    void Start()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // Follow the player perfectly on X while staying steady on Y and Z
        transform.position = new Vector3(playerTransform.position.x + aheadOffset, fixedY, fixedZ);
    }
}