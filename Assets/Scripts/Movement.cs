using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.3f;
    public Transform groundCheckPoint;

    [Header("Wall Run")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.6f;
    public float wallRunForce = 10f;
    public float wallRunGravity = 1f;
    public float maxWallRunTime = 1.5f;
    private float wallRunTimer;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchSpeed = 2.5f;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private bool isGrounded = false;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isWallRunning = false;
    private bool jumpQueued = false;
    private bool isCrouching = false;
    private Vector3 originalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        playerCollider = GetComponent<CapsuleCollider>();

        originalScale = transform.localScale;
    }

    void Update()
    {
        CheckGrounded();
        CheckWallRun();

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || isWallRunning))
        {
            jumpQueued = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StandUp();
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;

        if (isWallRunning)
            h = 0;

        Vector3 horizontalVelocity = move * (isCrouching ? crouchSpeed : moveSpeed);
        rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);

        if (jumpQueued)
        {
            jumpQueued = false;
            StopWallRun();

            rb.useGravity = true;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (isWallRunning)
        {
            rb.AddForce(transform.forward * wallRunForce, ForceMode.Acceleration);
            rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Acceleration);

            wallRunTimer -= Time.fixedDeltaTime;
            if (wallRunTimer <= 0f)
                StopWallRun();
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        Vector3 dashDirection = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        if (dashDirection == Vector3.zero)
            dashDirection = transform.forward;

        rb.velocity = dashDirection.normalized * dashSpeed;

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void CheckGrounded()
    {
        if (groundCheckPoint == null)
        {
            groundCheckPoint = transform;
        }

        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    void CheckWallRun()
    {
        if (isGrounded || isDashing) return;

        bool wallLeft = Physics.Raycast(transform.position, -transform.right, wallCheckDistance, wallLayer);
        bool wallRight = Physics.Raycast(transform.position, transform.right, wallCheckDistance, wallLayer);

        if ((wallLeft || wallRight) && Input.GetKey(KeyCode.W))
        {
            StartWallRun(wallLeft ? -transform.right : transform.right);
        }
        else
        {
            StopWallRun();
        }
    }

    void StartWallRun(Vector3 wallNormal)
    {
        if (isWallRunning) return;

        isWallRunning = true;
        wallRunTimer = maxWallRunTime;
        rb.useGravity = false;

        rb.velocity = Vector3.Project(rb.velocity, Vector3.Cross(wallNormal, Vector3.up));
    }

    void StopWallRun()
    {
        if (!isWallRunning) return;

        isWallRunning = false;
        rb.useGravity = true;
    }

    void Crouch()
    {
        if (playerCollider != null)
        {
            playerCollider.height = crouchHeight;
            Vector3 scale = originalScale;
            scale.y *= crouchHeight / standingHeight;
            transform.localScale = scale;
            isCrouching = true;
        }
    }

    void StandUp()
    {
        if (playerCollider != null)
        {
            playerCollider.height = standingHeight;
            transform.localScale = originalScale;
            isCrouching = false;
        }
    }
}
