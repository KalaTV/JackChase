using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 4f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float glideGravity = 2f;
    [SerializeField] private float normalGravity = 5f;
    [SerializeField] private float maxGrappleDistance = 10f;
    [SerializeField] private float grappleSpeed = 10f;
    [SerializeField] private KeyCode glideKey = KeyCode.LeftShift;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1.5f, 1f);
    [SerializeField] private LayerMask groundLayer;
    Rigidbody2D rb;
    [SerializeField] private bool isTouchingWall;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isWallSliding;

    public LayerMask layerMask;

    [SerializeField] private Transform player;
    [SerializeField] private LineRenderer lineRenderer;
    private Vector2 grapplePoint;
    private bool isGrappling = false;

    [SerializeField] private Transform player;
    [SerializeField] private LineRenderer lineRenderer;
    private Vector2 grapplePoint;
    private bool isGrappling = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wallJumpDirection.Normalize();
        rb.freezeRotation = true;
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        Move();
        Jump();
        Glide();
        CheckAround();
        Wallslide();
        Break();
        if (Input.GetMouseButtonDown(0))
        {
            TryGrapple();
        }

        if (Input.GetMouseButtonUp(0))
        {
            ReleaseGrapple();
        }

        if (isGrappling)
        {
            MoveTowardsGrapplePoint();
        }
    }

    private void TryGrapple()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, maxGrappleDistance, groundLayer);
        if (hit.collider != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            grapplePoint = mousePosition;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);
            lineRenderer.enabled = true;
            isGrappling = true;
        }
    }

    private void ReleaseGrapple()
        {
            lineRenderer.enabled = false;
            isGrappling = false;

        }
    

    private void MoveTowardsGrapplePoint()
        {
            if (isGrappling)
            {
                float step = grappleSpeed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, grapplePoint, step);

                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, grapplePoint);

                if (Vector2.Distance(transform.position, grapplePoint) < 0.1f)
                {
                    ReleaseGrapple();
                }
            }
        }

        private void Move()
        {
            float moveInput = Input.GetAxis("Horizontal") * speed;
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
        }

        private void CheckAround()
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

            isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right, 0.6f, groundLayer) ||
                             Physics2D.Raycast(transform.position, Vector2.left, 0.6f, groundLayer);
        }

        private void Wallslide()
        {
            if (isTouchingWall && rb.linearVelocity.y < 0)
            {
                isWallSliding = true;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlidingSpeed);
            }
            else
            {
                isWallSliding = false;
            }
        }

        private void Glide()
        {
            if (!isGrounded && Input.GetKey(glideKey))
            {
                rb.gravityScale = glideGravity;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -2f));
            }
            else
            {
                rb.gravityScale = normalGravity;
            }
        }

        private void Jump()
        {
            if (Input.GetKeyDown("space") && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
                Debug.Log("Jump");
            }
            else if (Input.GetKeyDown("space") && isTouchingWall)
            {
                float wallJumpDirectionX = isTouchingWall ? -Mathf.Sign(rb.linearVelocity.x) : 1;
                rb.linearVelocity = new Vector2(wallJumpDirectionX * wallJumpDirection.x * wallJumpForce,
                    wallJumpDirection.y * wallJumpForce);
            }
        }

        private void Break()
        {
            Vector2 direction = Vector2.right;
            if (transform.localScale.x < 0) // Si le perso est tourné à gauche
            {
                direction = Vector2.left;
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 3f);
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("break"))
                {
                    Debug.Log(" détecté !");

                    BreakingObject breakingObject = hit.collider.gameObject.GetComponent<BreakingObject>();
                    if (Input.GetKeyDown("q") && breakingObject != null)
                    {
                        breakingObject.Die();
                    }

                }

            }
        }
    }