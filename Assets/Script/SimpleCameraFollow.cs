using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform playerTransform;

    [Header("Positioning Adjustments")]
    [SerializeField] private float aheadDistance = 3f;   // How far ahead of the player the camera looks
    [SerializeField] private float heightOffset = 1f;    // How high up the camera sits

    [Header("Vertical Smoothness")]
    [SerializeField] private float ySmoothSpeed = 5f;    // How fast it catches up to jumps (higher = faster)

    private float targetZ;

    void Start()
    {
        // Save the camera's starting Z position (usually -10) so it doesn't clip into the 2D plane
        targetZ = transform.position.z;
    }

    // LateUpdate runs after the player has moved, preventing screen jittering
    void LateUpdate()
    {
        if (playerTransform == null) return;

        // 1. Lock X Position: Move perfectly in sync with the player + look ahead distance
        float targetX = playerTransform.position.x + aheadDistance;

        // 2. Smooth Y Position: Softly interpolate towards the player's height + offset
        float targetY = Mathf.Lerp(transform.position.y, playerTransform.position.y + heightOffset, ySmoothSpeed * Time.deltaTime);

        // 3. Apply the coordinates to the camera
        transform.position = new Vector3(targetX, targetY, targetZ);
    }
}