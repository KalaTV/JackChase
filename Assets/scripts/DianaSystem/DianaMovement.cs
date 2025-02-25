using UnityEngine;

public class DianaMovement : MonoBehaviour
{
    [Header("Références")]
    public Transform player;
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform frontCheck;

    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask environmentLayer;

    [Header("Déplacement & Collectibles")]
    public float followSpeed = 3f;
    public float desiredDistance = 10f;
    public float stepReduction = 1f;
    public int collectiblesThreshold = 5;
    public float escapeJumpDistance = 3f;
    public float escapeBoostSpeed = 7f; // 🔥 Nouvelle vitesse boostée au sol

    [Header("Adaptation aux obstacles")]
    public float boostedFollowSpeed = 5f;
    public float boostedJumpForce = 10f;
    public float obstacleDetectionRange = 1f;

    [Header("Saut & Wall Jump")]
    public float jumpForce = 7f;
    public float wallJumpForce = 15f;
    public Vector2 wallJumpDirection = new Vector2(1.5f, 1f);
    public float wallCheckDistance = 0.7f;
    public float jumpCooldown = 0.5f;

    private Rigidbody2D _rb;
    private Transform _myTransform;
    private bool _isJumping;
    private bool _isEscaping;
    private float _lastJumpTime;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _myTransform = transform;
        _rb.freezeRotation = true;
        _isJumping = false;
        _isEscaping = false;
        _lastJumpTime = -jumpCooldown;

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
        }
    }

    void FixedUpdate()
    {
        bool environmentObstacleDetected = CheckEnvironmentObstacle();
        bool isTouchingWall = IsTouchingWall();
        bool isGrounded = IsGrounded();

        float currentSpeed = environmentObstacleDetected ? boostedFollowSpeed : followSpeed;
        float targetX = player.position.x + desiredDistance;

        if (_isJumping) return;

        if (isTouchingWall && isGrounded)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return;
        }
        
        if (Vector2.Distance(player.position, _myTransform.position) < escapeJumpDistance && desiredDistance > 0)
        {
            _isEscaping = true;
            EscapeRun();
            return;
        }
        else
        {
            _isEscaping = false;
        }

        if (targetX > _myTransform.position.x)
        {
            Vector2 moveDirection = IsOnSlope() ? new Vector2(1, 0.3f).normalized : Vector2.right;
            _rb.gravityScale = IsOnSlope() ? 0.5f : 1f;
            _rb.linearVelocity = new Vector2(moveDirection.x * currentSpeed, _rb.linearVelocity.y);
        }
    }

    void Update()
    {
        bool isGroundAhead = IsGroundAhead();
        bool isGrounded = IsGrounded();
        bool isTouchingWall = IsTouchingWall();
        bool environmentObstacleDetected = CheckEnvironmentObstacle();
        bool canJump = Time.time > _lastJumpTime + jumpCooldown;

        if (_isJumping && isGrounded)
        {
            _isJumping = false;
        }

        if (!canJump || _isJumping) return;

        if (isTouchingWall && isGrounded)
        {
            _isJumping = true;
            _lastJumpTime = Time.time;
            WallJump();
            return;
        }
        
        if (!isGroundAhead && isGrounded)
        {
            _isJumping = true;
            _lastJumpTime = Time.time;
            Jump();
            return;
        }

        if (environmentObstacleDetected && isGrounded)
        {
            _isJumping = true;
            _lastJumpTime = Time.time;
            JumpBoosted();
        }
    }

    public void OnCollectibleCollected(int totalCollectibles)
    {
        if (totalCollectibles % collectiblesThreshold == 0)
        {
            desiredDistance = Mathf.Max(0, desiredDistance - stepReduction);
        }
    }

    void Jump()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
    }

    void JumpBoosted()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, boostedJumpForce);
    }

    void WallJump()
    {
        float wallDir = IsTouchingWallOnRight() ? -1f : 1f;
        Vector2 jumpDir = new Vector2(wallDir * wallJumpDirection.x, wallJumpDirection.y).normalized;
        _rb.linearVelocity = new Vector2(Mathf.Abs(jumpDir.x) * followSpeed, jumpDir.y * wallJumpForce);
    }

    void EscapeRun()
    {
        Vector2 moveDirection = IsOnSlope() ? new Vector2(1, 0.3f).normalized : Vector2.right;
        _rb.linearVelocity = new Vector2(moveDirection.x * escapeBoostSpeed, _rb.linearVelocity.y);
    }

    bool CheckEnvironmentObstacle()
    {
        return Physics2D.Raycast(frontCheck.position, _myTransform.right, obstacleDetectionRange, environmentLayer).collider != null;
    }

    bool IsGroundAhead()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer).collider != null;
    }

    bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.3f, groundLayer).collider != null;
    }

    bool IsTouchingWall()
    {
        return Physics2D.Raycast(wallCheck.position, _myTransform.right, wallCheckDistance, wallLayer).collider != null ||
               Physics2D.Raycast(wallCheck.position, -_myTransform.right, wallCheckDistance, wallLayer).collider != null;
    }

    bool IsTouchingWallOnRight()
    {
        return Physics2D.Raycast(wallCheck.position, _myTransform.right, wallCheckDistance, wallLayer).collider != null;
    }

    bool IsOnSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        return hit.collider != null && Vector2.Angle(hit.normal, Vector2.up) > 10f;
    }
}

