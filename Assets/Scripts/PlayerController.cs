using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Dynamic Forward Movement")]
    [SerializeField] private float startSpeed = 5f;
    [SerializeField] private float maxSpeed = 14f;
    [SerializeField] private float speedIncreaseRate = 0.02f;
    [SerializeField] private float speedTransitionSmoothness = 15f;

    [Header("Precision Jumping Settings")]
    [SerializeField] private float jumpForce = 13f;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Controllable Arc Modifiers")]
    [Range(0f, 1f)]
    [SerializeField] private float jumpCutMultiplier = 0.4f;
    [SerializeField] private float fallMultiplier = 3.0f;
    [SerializeField] private float lowJumpMultiplier = 2.0f;

    [Header("Sliding Settings (Enhanced Speed)")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideSpeedMultiplier = 1.35f;

    [Header("Invincibility Frames")]
    [SerializeField] private float iFrameDuration = 1.5f;
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    // Internal State Flags
    private float masterProgressionSpeed;
    private float actualAppliedSpeed;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool isSliding;
    private bool isAirSliding;
    private float slideTimer;
    private bool isHoldingJump;

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
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColliderSize = boxCollider.size;
        originalColliderOffset = boxCollider.offset;

        masterProgressionSpeed = startSpeed;
        actualAppliedSpeed = startSpeed;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        // 1. Difficulty Speed Scaling Over Time
        if (masterProgressionSpeed < maxSpeed)
        {
            masterProgressionSpeed += speedIncreaseRate * Time.deltaTime;
        }

        // 2. Continuous Ground Check Tracking
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        // 3. Cache the Input State Perfectly Every Single Frame
        isHoldingJump = Input.GetKey(KeyCode.UpArrow);

        // 4. Precision Horizontal Velocity Interpolation
        float targetXVelocity = isSliding ? (masterProgressionSpeed * slideSpeedMultiplier) : masterProgressionSpeed;
        actualAppliedSpeed = Mathf.MoveTowards(actualAppliedSpeed, targetXVelocity, speedTransitionSmoothness * Time.deltaTime);

        // 5. Jump Trigger Input Window
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isSliding)
        {
            if (isGrounded)
            {
                ExecuteInitialJump();
            }
            else if (canDoubleJump)
            {
                ExecuteInitialJump();
                canDoubleJump = false;
            }
        }

        // 6. INSTANT VELOCITY CUTOFF
        if (Input.GetKeyUp(KeyCode.UpArrow) && rb.linearVelocity.y > 0 && !isAirSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        // 7. Slide Control Mechanics (Ground & Air)
        // Ground Slide (Down Arrow)
        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded && !isSliding)
        {
            StartSlide(false);
        }

        // Air Slide (Right Arrow while airborne)
        if (Input.GetKeyDown(KeyCode.RightArrow) && !isGrounded && !isSliding)
        {
            StartSlide(true);
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                StopSlide();
            }
        }

        // 8. Visual Parameter Synced updates
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isSliding", isSliding);
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // If air sliding, lock Y velocity to 0 so the player floats perfectly straight through air tunnels
        if (isAirSliding)
        {
            rb.linearVelocity = new Vector2(actualAppliedSpeed, 0f);
            return;
        }

        // Apply our normal horizontal run movement profile
        rb.linearVelocity = new Vector2(actualAppliedSpeed, rb.linearVelocity.y);

        // Advanced Gravity Weight Management
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !isHoldingJump)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void ExecuteInitialJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // Now takes a parameter to know if it's an air execution
    void StartSlide(bool airSlide)
    {
        isSliding = true;
        isAirSliding = airSlide;
        slideTimer = slideDuration;

        // Cut hitbox height in half instantly (works perfectly in mid-air tunnels too!)
        boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
        boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y * 0.25f));

        if (isAirSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Halt vertical falling instantly
        }
    }

    void StopSlide()
    {
        isSliding = false;
        isAirSliding = false;
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

    public void HitPlayer(int damageValue)
    {
        if (isInvincible) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.DamagePlayer(damageValue);
        }

        StartCoroutine(IFrameBlinkRoutine());
    }

    /// Drastically drops the player's speed as a physical penalty for hitting sharp hazards.
    public void ApplySpeedPenalty(float penaltyFactor)
    {
        // Instantly cut the current speed down (e.g., matching startSpeed or a percentage)
        actualAppliedSpeed = Mathf.Max(startSpeed, actualAppliedSpeed * penaltyFactor);

        Debug.Log("Seryn tripped on spikes! Speed penalized.");
    }

    private System.Collections.IEnumerator IFrameBlinkRoutine()
    {
        isInvincible = true;
        float elapsed = 0f;
        float blinkInterval = 0.15f;

        while (elapsed < iFrameDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }
}