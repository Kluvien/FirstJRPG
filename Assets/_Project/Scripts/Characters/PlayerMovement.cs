using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveDirection = Vector2.down;
    private bool canMove = true;
    private string currentAnimation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            PlayIdleAnimation();
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(horizontal, vertical).normalized;

        if (moveInput != Vector2.zero)
        {
            lastMoveDirection = moveInput;
            PlayWalkAnimation();
        }
        else
        {
            PlayIdleAnimation();
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!canMove)
        {
            moveInput = Vector2.zero;
            PlayIdleAnimation();
        }
    }

    public void PlayCutsceneMoveAnimation(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            PlayIdleAnimation();
            return;
        }

        lastMoveDirection = direction.normalized;
        PlayWalkAnimation();
    }

    public void PlayCutsceneIdleAnimation()
    {
        PlayIdleAnimation();
    }

    private void PlayWalkAnimation()
    {
        if (Mathf.Abs(lastMoveDirection.x) > Mathf.Abs(lastMoveDirection.y))
        {
            PlayAnimation("PlayerWalkSide");
            SetSideFlip();
        }
        else if (lastMoveDirection.y > 0f)
        {
            PlayAnimation("PlayerWalkUp");
        }
        else
        {
            PlayAnimation("PlayerWalkDown");
        }
    }

    private void PlayIdleAnimation()
    {
        if (Mathf.Abs(lastMoveDirection.x) > Mathf.Abs(lastMoveDirection.y))
        {
            PlayAnimation("PlayerIdleSide");
            SetSideFlip();
        }
        else if (lastMoveDirection.y > 0f)
        {
            PlayAnimation("PlayerIdleUp");
        }
        else
        {
            PlayAnimation("PlayerIdleDown");
        }
    }

    private void SetSideFlip()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        // Sprite side dasar Anda menghadap kiri.
        spriteRenderer.flipX = lastMoveDirection.x > 0f;
    }

    private void PlayAnimation(string animationName)
    {
        if (animator == null || currentAnimation == animationName)
        {
            return;
        }

        currentAnimation = animationName;
        animator.Play(animationName);
    }
}