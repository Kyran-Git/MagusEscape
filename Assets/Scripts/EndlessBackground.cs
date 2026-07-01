using UnityEngine;

public class EndlessBackground : MonoBehaviour
{
    [Header("Loop Settings")]
    [SerializeField] private int totalTilesInChain = 3;

    private SpriteRenderer spriteRenderer;
    private float backgroundWidth;
    private float camLeftEdge;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        backgroundWidth = spriteRenderer.bounds.size.x;

        camLeftEdge = Camera.main.transform.position.x
            - (Camera.main.orthographicSize * Camera.main.aspect);
    }

    void Update()
    {
        float speed = WorldManager.Instance != null ? WorldManager.Instance.currentWorldSpeed : 5f;
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // bounds.max.x is the sprite's actual right edge in world space,
        // correct no matter where the pivot is set.
        if (spriteRenderer.bounds.max.x <= camLeftEdge)
        {
            float shiftDistance = backgroundWidth * totalTilesInChain;
            transform.position += new Vector3(shiftDistance, 0f, 0f);
        }
    }
}