using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jumping Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Snappy Fall Physics")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Sliding Settings")]
    [SerializeField] private float slideDuration = 0.6f;

    private bool isGrounded;
    private bool canDoubleJump;
    private bool isSliding;
    private float slideTimer;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        // Save the normal size of the character's hitbox
        originalColliderSize = boxCollider.size;
        originalColliderOffset = boxCollider.offset;
    }

    void Update()
    {
        // 1. Check if Seryn is touching the ground
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        // 2. Jump Input (Up Arrow)
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isSliding)
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (canDoubleJump) // Optional Double-Jump capability
            {
                Jump();
                canDoubleJump = false;
            }
        }

        // 3. Slide Input (Down Arrow)
        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded && !isSliding)
        {
            StartSlide();
        }

        // 4. Handle Slide Timer
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                StopSlide();
            }
        }

        // 5. Update the Animator Parameters
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isSliding", isSliding);
    }

    void FixedUpdate()
    {
        // Stationary Rule: Seryn does not move forward or backward on the X axis!
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // Physics Tweak: Makes gravity pull her down faster so jumping doesn't feel floaty
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

        // Cut the height of the collider box in half so she fits under high obstacles
        boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
        boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y * 0.25f));
    }

    void StopSlide()
    {
        isSliding = false;

        // Return collider to normal size when slide ends
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