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
    public float wallRunGravity = 1f;  // Adjust this value for slower fall
    public float maxWallRunTime = 1.5f;  // Max time player can wall run
    private float wallRunTimer;

    private Rigidbody rb;
    private bool isGrounded = false;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isWallRunning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckGrounded();
        CheckWallRun();

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || isWallRunning))
        {
            StopWallRun();
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);  // Reset Y velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        Vector3 velocity = move * moveSpeed;
        velocity.y = rb.velocity.y;  // Preserve Y velocity (no sudden changes)
        rb.velocity = velocity;

        if (isWallRunning)
        {
            rb.AddForce(transform.forward * wallRunForce, ForceMode.Acceleration);
            rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Acceleration);  // Apply custom gravity

            // Decrease the wall run timer
            wallRunTimer -= Time.fixedDeltaTime;
            if (wallRunTimer <= 0f)
            {
                StopWallRun();  // Stop wall running when the timer runs out
            }
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
            // fallback to object center if no ground check point set
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
        wallRunTimer = maxWallRunTime;  // Reset the timer each time wall run starts
        rb.useGravity = false;  // Disable normal gravity during wall run

        rb.velocity = Vector3.Project(rb.velocity, Vector3.Cross(wallNormal, Vector3.up));  // Move player along the wall
    }

    void StopWallRun()
    {
        if (!isWallRunning) return;

        isWallRunning = false;
        rb.useGravity = true;  // Re-enable normal gravity when not wall running
    }
}
