using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float speedIncreaseRate = 0.05f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool isGrounded;
    private bool canDoubleJump;

    [Header("Responsive Physics Tweaks")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Sliding")]
    [SerializeField] private float slideDuration = 0.6f;
    private bool isSliding;
    private float slideTimer;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        originalColliderSize = boxCollider.size;
        originalColliderOffset = boxCollider.offset;
    }

    void Update()
    {
        // 1. Dynamic Difficulty
        if (forwardSpeed < maxSpeed)
        {
            forwardSpeed += speedIncreaseRate * Time.deltaTime;
        }

        // 2. Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        // 3. Jump Input (Up Arrow)
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isSliding)
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (canDoubleJump)
            {
                Jump();
                canDoubleJump = false;
            }
        }

        // 4. Slide Input (Down Arrow)
        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded && !isSliding)
        {
            StartSlide();
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                StopSlide();
            }
        }
    }

    void FixedUpdate()
    {
        // Automated forward velocity combined with current vertical movement
        rb.linearVelocity = new Vector2(forwardSpeed, rb.linearVelocity.y);

        // Responsive Jump Physics: Pulls player down quicker for snappy movement
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.UpArrow))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        // Shrink collider to pass under high obstacles
        boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
        boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y * 0.25f));
    }

    void StopSlide()
    {
        isSliding = false;
        boxCollider.size = originalColliderSize;
        boxCollider.offset = originalColliderOffset;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}