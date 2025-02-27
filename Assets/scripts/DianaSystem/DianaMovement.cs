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
    public float followSpeed = 6f;
    public float desiredDistance = 8f;
    public float stepReduction = 1f;
    public int collectiblesThreshold = 5;
    public float escapeBoostSpeed = 10f;
    private int collectedItems = 0;

    [Header("Adaptation aux obstacles")]
    public float boostedFollowSpeed = 8f;
    public float obstacleDetectionRange = 3f;
    
    private float _stuckTime = 0f;
    private float _stuckThreshold = 0.7f;
    private float _unstuckJumpForce = 5f;

    [Header("Saut & Wall Jump")]
    public float jumpForce = 6f;
    public float maxJumpForce = 16f;
    public float jumpCooldown = 0.5f;
    public float fallMultiplier = 4.5f;  
    public float maxVerticalSpeed = 5f;  

    private Rigidbody2D _rb;
    private Transform _myTransform;
    private bool _isJumping;
    private float _lastJumpTime;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _myTransform = transform;
        _rb.freezeRotation = true;
        _isJumping = false;
        _lastJumpTime = -jumpCooldown;
        collectedItems = 0;

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
        bool isGrounded = IsGrounded();
        bool isOnSlope = IsOnSlope();
        bool isTouchingWall = IsTouchingWall();
        bool environmentObstacleAhead = CheckEnvironmentObstacleAhead();
        bool isObstacleTooHigh = IsObstacleTooHigh();

        float currentSpeed = environmentObstacleAhead ? boostedFollowSpeed : followSpeed;
        float targetX = player.position.x + desiredDistance;

        if (_isJumping) return;
        
        if (!isGrounded && _rb.linearVelocity.y < 0)
        {
            _rb.linearVelocity += Vector2.down * (fallMultiplier * Time.fixedDeltaTime);
        }
        
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Clamp(_rb.linearVelocity.y, -maxVerticalSpeed, maxVerticalSpeed));
        
        if (isGrounded && isTouchingWall && Mathf.Abs(_rb.linearVelocity.x) < 0.1f)
        {
            _stuckTime += Time.fixedDeltaTime;
            if (_stuckTime > _stuckThreshold)
            {
                _stuckTime = 0;
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _unstuckJumpForce);
            }
        }
        else
        {
            _stuckTime = 0;
        }
        
        if (environmentObstacleAhead && isGrounded && isObstacleTooHigh)
        {
            float obstacleHeight = GetObstacleHeight();
            float adjustedJumpForce = Mathf.Clamp(obstacleHeight * 1.3f, jumpForce, maxJumpForce);
            Jump(adjustedJumpForce);
            return;
        }
        
        if (environmentObstacleAhead && !isObstacleTooHigh)
        {
            return;
        }
        
        if (Vector2.Distance(player.position, _myTransform.position) < desiredDistance)
        {
            _rb.linearVelocity = new Vector2(escapeBoostSpeed, _rb.linearVelocity.y);
            return;
        }
        
        if (targetX > _myTransform.position.x)
        {
            Vector2 moveDirection = isOnSlope ? new Vector2(1, 0.3f).normalized : Vector2.right;
            _rb.gravityScale = isOnSlope ? 0.5f : 1f;
            _rb.linearVelocity = new Vector2(moveDirection.x * currentSpeed, _rb.linearVelocity.y);
        }
    }

    void Update()
    {
        bool isGrounded = IsGrounded();
        bool canJump = Time.time > _lastJumpTime + jumpCooldown;

        if (_isJumping && isGrounded)
        {
            _isJumping = false;
        }

        if (!canJump || _isJumping) return;
    }

    public void OnCollectibleCollected(int totalCollectibles)
    {
        collectedItems = totalCollectibles;
        if (collectedItems % collectiblesThreshold == 0)
        {
            desiredDistance = Mathf.Max(0, desiredDistance - stepReduction);
        }
    }

    void Jump(float force)
    {
        _isJumping = true;
        _lastJumpTime = Time.time;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, force);
    }

    bool CheckEnvironmentObstacleAhead()
    {
        return Physics2D.Raycast(frontCheck.position, _myTransform.right, obstacleDetectionRange, environmentLayer).collider != null;
    }

    bool IsObstacleTooHigh()
    {
        RaycastHit2D hit = Physics2D.Raycast(frontCheck.position, Vector2.up, 1f, environmentLayer);
        return hit.collider != null;
    }

    float GetObstacleHeight()
    {
        RaycastHit2D hit = Physics2D.Raycast(frontCheck.position, Vector2.up, 3f, environmentLayer);
        return hit.collider != null ? hit.distance : jumpForce;
    }

    bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.3f, groundLayer).collider != null;
    }

    bool IsOnSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        return hit.collider != null && Vector2.Angle(hit.normal, Vector2.up) > 10f;
    }

    bool IsTouchingWall()
    {
        return Physics2D.Raycast(wallCheck.position, _myTransform.right, 0.5f, wallLayer).collider != null;
    }
}