using UnityEngine;

public class PlayerRunner : MonoBehaviour
{
    [Header("Forward Movement")]
    public float forwardSpeed = 6f;

    [Header("Lane Movement")]
    public float laneDistance = 3f;
    public float laneChangeSpeed = 10f;
    public float laneRepeatDelay = 0.25f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Animator animator;

    private float jumpClipLength;

    private int currentLane = 1;

    private bool isGrounded;
    private bool jumpLocked;
    private bool hasLeftGround;
    private bool jumping;

    private float laneTimer;
    private int heldDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.applyRootMotion = false;

            if (animator.runtimeAnimatorController != null)
            {
                foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == "jump.com")
                    {
                        jumpClipLength = clip.length;
                        break;
                    }
                }
            }
        }
    }

    private void Start()
    {
        if (animator != null)
        {
            animator.SetBool("IsRunning", true);
        }
    }

    private void Update()
    {
        CheckGround();
        HandleLaneInput();

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;

        // Player always keeps moving forward
        velocity.z = forwardSpeed;

        // Target lane
        float targetX = (currentLane - 1) * laneDistance;

        float newX = Mathf.MoveTowards(
            rb.position.x,
            targetX,
            laneChangeSpeed * Time.fixedDeltaTime
        );

        velocity.x =
            (newX - rb.position.x) / Time.fixedDeltaTime;

        // Never touch Y here
        rb.linearVelocity = velocity;
    }

    private void CheckGround()
    {
        Vector3 origin =
            capsule.bounds.center + Vector3.up * 0.05f;

        float distance =
            capsule.bounds.extents.y + 0.12f;

        isGrounded = Physics.Raycast(
            origin,
            Vector3.down,
            distance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        // Player actually left ground
        if (!isGrounded && jumping)
        {
            hasLeftGround = true;
        }

        // Landed
        if (jumping &&
            hasLeftGround &&
            isGrounded &&
            rb.linearVelocity.y <= 0.1f)
        {
            jumping = false;
            jumpLocked = false;
            hasLeftGround = false;

            if (animator != null)
            {
                animator.ResetTrigger("Jump");
                animator.SetBool("IsJumping", false);
            }
        }
    }

    private void Jump()
    {
        if (jumpLocked)
            return;

        if (!isGrounded)
            return;

        jumpLocked = true;
        jumping = true;
        hasLeftGround = false;

        // Remove previous vertical velocity
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        // ONE real physical jump
        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );

        // Play the non-looping clip once over the expected physical flight.
        // Actual ground detection, not clip exit time, controls the return to run.
        if (animator != null)
        {
            float flightTime = 2f * (jumpForce / rb.mass) /
                Mathf.Max(0.01f, -Physics.gravity.y);
            float jumpSpeed = jumpClipLength > 0f && flightTime > 0f
                ? jumpClipLength / flightTime
                : 1f;

            animator.SetFloat("JumpSpeed", jumpSpeed);
            animator.SetBool("IsJumping", true);
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
        }
    }

    private void HandleLaneInput()
    {
        int direction = 0;

        if (Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.A))
        {
            direction = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow) ||
                 Input.GetKey(KeyCode.D))
        {
            direction = 1;
        }

        if (direction == 0)
        {
            heldDirection = 0;
            laneTimer = 0f;
            return;
        }

        if (heldDirection != direction)
        {
            heldDirection = direction;

            ChangeLane(direction);

            laneTimer = laneRepeatDelay;

            return;
        }

        laneTimer -= Time.deltaTime;

        if (laneTimer <= 0f)
        {
            ChangeLane(direction);

            laneTimer = laneRepeatDelay;
        }
    }

    private void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(
            currentLane + direction,
            0,
            2
        );
    }
}
