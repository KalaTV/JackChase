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

    [Header("Adaptation aux obstacles")]
    public float boostedFollowSpeed = 5f;
    public float boostedJumpForce = 10f;
    public float obstacleDetectionRange = 1f;

    [Header("Saut & Wall Jump")]
    public float jumpForce = 7f;
    public float wallJumpForce = 15f;
    public Vector2 wallJumpDirection = new Vector2(2f, 1.2f);
    public float wallCheckDistance = 0.7f;

    private Rigidbody2D _rb;
    private Transform _myTransform;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _myTransform = transform;
        _rb.freezeRotation = true;

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
        
        float currentSpeed = environmentObstacleDetected ? boostedFollowSpeed : followSpeed;
        float targetX = player.position.x + desiredDistance;

        if (isTouchingWall)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return; 
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

        if (isTouchingWall && isGrounded)
        {
            WallJump();
            return;
        }

        if ((!isGroundAhead || environmentObstacleDetected) && isGrounded)
        {
            if (environmentObstacleDetected)
            {
                JumpBoosted();
            }
            else
            {
                Jump();
            }
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
        _rb.linearVelocity = jumpDir * wallJumpForce;
    }

    bool CheckEnvironmentObstacle()
    {
        RaycastHit2D hit = Physics2D.Raycast(frontCheck.position, _myTransform.right, obstacleDetectionRange, environmentLayer);
        return hit.collider != null;
    }

    bool IsGroundAhead()
    {
        float rayDistance = 1f;
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, groundLayer);
        return hit.collider != null;
    }

    bool IsGrounded()
    {
        float rayDistance = 0.3f;
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, groundLayer);
        return hit.collider != null;
    }

    bool IsTouchingWall()
    {
        float checkDistance = wallCheckDistance;
        Vector2 origin = wallCheck.position;
        return Physics2D.Raycast(origin, _myTransform.right, checkDistance, wallLayer).collider != null ||
               Physics2D.Raycast(origin, -_myTransform.right, checkDistance, wallLayer).collider != null;
    }

    bool IsTouchingWallOnRight()
    {
        return Physics2D.Raycast(wallCheck.position, _myTransform.right, wallCheckDistance, wallLayer).collider != null;
    }

    bool IsOnSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        if (hit.collider != null)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            return angle > 10f;
        }
        return false;
    }
}
