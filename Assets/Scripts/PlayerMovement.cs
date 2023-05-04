using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 7f;
    [SerializeField] float jumpHeight = 14f;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private LayerMask jumpableWall;
    [SerializeField] private LayerMask jumpablePlatform;
    [SerializeField] private AudioSource soundJump;
    [SerializeField] private AudioSource soundDoubleJump;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D playerBody;
    private Animator playerAnimation;
    private SpriteRenderer playerSprite;
    private BoxCollider2D playerBoxCollider;
    private float directionX = 0f;
    private enum MovementState { idle, running, jumping, falling, double_jump, wall_hold, dashing };
    private bool canDoubleJump = false;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);
    private bool isFacingRight = true;

    [Header("Dashing")]
    [SerializeField] private float dashingVelocity = 14f;
    [SerializeField] private float dashingTime = 0.5f;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool canDash = true;
    private TrailRenderer trailRenderer;


    // Start is called before the first frame update
    private void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerBoxCollider = GetComponent<BoxCollider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerBody.bodyType != RigidbodyType2D.Static)
        {
            directionX = Input.GetAxisRaw("Horizontal");
            ControlPlayer();
            UpdateAnimation();
            WallSlide();
            WallJump();

            if (!isWallJumping)
            {
                Flip();
            }
        }
    }

    private void ControlPlayer()
    {
        // left right movement
        playerBody.velocity = new Vector2(directionX * speed, playerBody.velocity.y);

        //normal jump
        if (Input.GetButtonDown("Jump") && (IsGrounded() || canDoubleJump || isWalled()))
        {
            if (canDoubleJump)
            {
                soundJump.Play();
            }
            else
            {
                soundDoubleJump.Play();
            }
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpHeight);
            canDoubleJump = !canDoubleJump;
        }

        //decrease jump height when just tapping jump button
        if (Input.GetButtonUp("Jump") && playerBody.velocity.y > .01f)
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, playerBody.velocity.y * 0.5f);
        }

        //check dashing
        if (Input.GetButtonDown("Dash") && canDash)
        {
            isDashing = true;
            canDash = false;
            trailRenderer.emitting = true;
            dashingDir = new Vector2(directionX, Input.GetAxisRaw("Vertical"));
            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }

            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            playerBody.velocity = dashingDir.normalized * dashingVelocity;
            return;
        }
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (directionX > 0f)
        {
            state = MovementState.running;
        }
        else if (directionX < 0f)
        {
            state = MovementState.running;
        }
        else
        {
            state = MovementState.idle;
        }

        //values is never zero, checking Y speed
        if (playerBody.velocity.y > .01f)
        {
            if (!canDoubleJump)
            {
                state = MovementState.double_jump;
            }
            else
            {
                state = MovementState.jumping;
            }
        }
        else if (playerBody.velocity.y < -.01f)
        {
            state = MovementState.falling;
        }

        if (isWallSliding)
        {
            state = MovementState.wall_hold;
        }

        if (isDashing)
        {
            state = MovementState.dashing;
        }

        playerAnimation.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        bool isGrounded = Physics2D.BoxCast(playerBoxCollider.bounds.center, playerBoxCollider.bounds.size, 0f, Vector2.down, .1f, jumpableGround);

        if(!isGrounded)
            isGrounded = Physics2D.BoxCast(playerBoxCollider.bounds.center, playerBoxCollider.bounds.size, 0f, Vector2.down, .1f, jumpableWall);

        if(!isGrounded)
            isGrounded = Physics2D.BoxCast(playerBoxCollider.bounds.center, playerBoxCollider.bounds.size, 0f, Vector2.down, .1f, jumpablePlatform);

        if (isGrounded)
        {
            canDash = true;
            canDoubleJump = false;
        }
        return isGrounded;
    }

    private bool isWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (isWalled() && !IsGrounded() && directionX != 0f)
        {
            isWallSliding = true;
            playerBody.velocity = new Vector2(playerBody.velocity.x, Mathf.Clamp(playerBody.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            playerBody.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && directionX < 0f || !isFacingRight && directionX > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator StopDashing() {
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        isDashing = false;
    }
}
