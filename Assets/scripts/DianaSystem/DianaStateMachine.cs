using UnityEngine;
public class DianaStateMachine : MonoBehaviour
{
    public enum DianaState
    {
        Following,
        Jumping,
        BoostedJump,
        WallJumping
    }

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
    public float obstacleDetectionRange = 1f;

    [Header("Saut & Wall Jump")]
    public float jumpForce = 7f;
    public float boostedJumpForce = 10f;
    public float wallJumpForce = 7f;
    public Vector2 wallJumpDirection = new Vector2(1f, 1f);
    public float wallCheckDistance = 0.5f;

    private Rigidbody2D _rb;
    private Transform _myTransform;
    public DianaState currentState = DianaState.Following;
    
    private const float GroundRayDistance = 1f;
    private const float GroundedRayDistance = 0.1f;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _myTransform = transform;
    }

    void Update()
    {
        bool groundAhead = IsGroundAhead();
        bool grounded = IsGrounded();
        bool touchingWall = IsTouchingWall();
        bool envObstacle = CheckEnvironmentObstacle();

        if (grounded && !groundAhead)
        {
            if (touchingWall)
                currentState = DianaState.WallJumping;
            else if (envObstacle)
                currentState = DianaState.BoostedJump;
            else
                currentState = DianaState.Jumping;
        }
        else
        {
            currentState = DianaState.Following;
        }

        switch (currentState)
        {
            case DianaState.Following:
                FollowPlayer();
                break;
            case DianaState.Jumping:
                Jump();
                break;
            case DianaState.BoostedJump:
                JumpBoosted();
                break;
            case DianaState.WallJumping:
                WallJump();
                break;
        }
    }
    void FollowPlayer()
    {
        float targetX = player.position.x + desiredDistance;
    
        if (targetX < _myTransform.position.x)
            targetX = _myTransform.position.x;
        Vector2 targetPos = new Vector2(targetX, _myTransform.position.y);
        _myTransform.position = Vector2.MoveTowards(_myTransform.position, targetPos, followSpeed * Time.deltaTime);
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
    public void OnCollectibleCollected(int totalCollectibles)
    {
        if (totalCollectibles % collectiblesThreshold == 0)
        {
            desiredDistance = Mathf.Max(0, desiredDistance - stepReduction);
        }
    }
    bool IsGroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, GroundRayDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector2.down * GroundRayDistance, Color.red);
        return hit.collider != null;
    }
    bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, GroundedRayDistance, groundLayer).collider != null;
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
    bool CheckEnvironmentObstacle()
    {
        RaycastHit2D hit = Physics2D.Raycast(frontCheck.position, _myTransform.right, obstacleDetectionRange, environmentLayer);
        Debug.DrawRay(frontCheck.position, _myTransform.right * obstacleDetectionRange, Color.yellow);
        return hit.collider != null;
    }
}
