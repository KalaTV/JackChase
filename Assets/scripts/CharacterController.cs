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
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1.5f, 1f);
    [SerializeField] private LayerMask groundLayer;
    Rigidbody2D rb;
    [SerializeField] private bool isTouchingWall;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isWallSliding;
    public LayerMask layerMask;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wallJumpDirection.Normalize();
    }

    private void Update()
    {
        Move();
        Jump();
        CheckAround();
        Wallslide();
        Break();
    }

    private void Move()
    {
        float moveInput = Input.GetAxis("Horizontal") * speed;
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    private void CheckAround()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);

        isTouchingWall = Physics2D.Raycast(transform.position, Vector2.right, 0.6f, groundLayer)|| Physics2D.Raycast(transform.position, Vector2.left, 0.6f, groundLayer);
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
            rb.linearVelocity = new Vector2(wallJumpDirectionX * wallJumpDirection.x * wallJumpForce, wallJumpDirection.y * wallJumpForce);
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
        if (hit.collider != null){
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