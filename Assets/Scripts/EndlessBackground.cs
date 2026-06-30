using UnityEngine;

public class EndlessBackground : MonoBehaviour
{
    [Header("Loop Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private int totalTilesInChain = 2; // Set this to 2 or 4 

    private float backgroundWidth;
    private float leftBoundary;

    void Start()
    {
        // 1. Calculate the width of this specific background sprite
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;

        // 2. Teleport when the tile completely clears the left side of the screen
        leftBoundary = -backgroundWidth;
    }

    void Update()
    {
        // 3. Constantly scroll the world to the left past our stationary player
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 4. Multi-Tile Leapfrog: Teleport to the very end of the line
        if (transform.position.x <= leftBoundary)
        {
            // Shifting forward by (Width * Total Tiles) preserves the alternating pattern perfectly
            float shiftDistance = backgroundWidth * totalTilesInChain;
            transform.position += new Vector3(shiftDistance, 0f, 0f);
        }
    }
}