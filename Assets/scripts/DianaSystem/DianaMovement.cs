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
    public float wallJumpForce = 7f;
    public Vector2 wallJumpDirection = new Vector2(1, 1);
    public float wallCheckDistance = 0.5f;

    private Rigidbody2D _rb;
    private Transform _myTransform;         

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _myTransform = transform;
    }

    void Update()
    {
        bool environmentObstacleDetected = frontCheck && CheckEnvironmentObstacle();

        float currentSpeed = environmentObstacleDetected ? boostedFollowSpeed : followSpeed;

        float targetX = player.position.x + desiredDistance;
     
        if (targetX < _myTransform.position.x)
            targetX = _myTransform.position.x;
        Vector2 targetPosition = new Vector2(targetX, _myTransform.position.y);
        _myTransform.position = Vector2.MoveTowards(_myTransform.position, targetPosition, currentSpeed * Time.deltaTime);

        if (!IsGroundAhead() && IsGrounded())
        {
            if (IsTouchingWall())
                WallJump();
            else if (environmentObstacleDetected)
                JumpBoosted();
            else
                Jump();
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
        Debug.DrawRay(frontCheck.position, _myTransform.right * obstacleDetectionRange, Color.yellow);
        return hit.collider != null;
    }

    bool IsGroundAhead()
    {
        float rayDistance = 1f;
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector2.down * rayDistance, Color.red);
        return hit.collider != null;
    }
    bool IsGrounded()
    {
        float rayDistance = 0.1f;
        return Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, groundLayer).collider != null;
    }
    bool IsTouchingWall()
    {
        Vector2 origin = wallCheck.position;
        if (Physics2D.Raycast(origin, _myTransform.right, wallCheckDistance, wallLayer).collider != null)
            return true;
        return Physics2D.Raycast(origin, -_myTransform.right, wallCheckDistance, wallLayer).collider != null;
    }
    bool IsTouchingWallOnRight()
    {
        return Physics2D.Raycast(wallCheck.position, _myTransform.right, wallCheckDistance, wallLayer).collider != null;
    }
}
