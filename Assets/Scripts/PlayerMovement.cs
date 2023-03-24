using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 7f;
    [SerializeField] float jumpHeight = 14f;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private AudioSource soundJump;
    [SerializeField] private AudioSource soundDoubleJump;

    private Rigidbody2D playerBody;
    private Animator playerAnimation;
    private SpriteRenderer playerSprite;
    private BoxCollider2D playerBoxCollider;
    private float directionX = 0f;
    private enum MovementState { idle, running, jumping, falling, double_jump };
    private bool canDoubleJump = false;


    // Start is called before the first frame update
    private void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerBoxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (playerBody.bodyType != RigidbodyType2D.Static)
        {
            directionX = Input.GetAxisRaw("Horizontal");
            ControlPlayer();
            UpdateAnimation();
        }
    }

    private void ControlPlayer()
    {
        // left right movement
        playerBody.velocity = new Vector2(directionX * speed, playerBody.velocity.y);

        //normal jump
        if (Input.GetButtonDown("Jump") && (IsGrounded() || canDoubleJump))
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
    }

    private void UpdateAnimation()
    {
        MovementState state;

        if (directionX > 0f)
        {
            state = MovementState.running;
            playerSprite.flipX = false;
        }
        else if (directionX < 0f)
        {
            state = MovementState.running;
            playerSprite.flipX = true;
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

        playerAnimation.SetInteger("state", (int)state);
    }

    private bool IsGrounded()
    {
        bool isGrounded = Physics2D.BoxCast(playerBoxCollider.bounds.center, playerBoxCollider.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        if (isGrounded)
        {
            canDoubleJump = false;
        }
        return isGrounded;
    }
}
