using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 10f;

    [Header("Jump Settings")]
    [SerializeField] private LayerMask jumpableGround;

    [Header("Slope Handling")]
    public float slopeSlidingSpeed = 5f;
    private bool onSlope = false;
    private Vector2 slopeNormalPerp;
    private float slopeAngle = 0f;
    private RaycastHit2D slopeHit;

    [Header("UI & Audio")]
    public GameObject finishMenuUI;
    public AudioSource deathSource;
    public AudioSource finishSource;
    public AudioSource bounceSource;

    private AudioSource footstepAudio;

    // Components
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private BoxCollider2D coll;
    private PlayerController playerController;

    // Movement variables
    private Vector2 moveInput;
    private float mobileInputX = 0f;
    private float mobileInputY = 0f;
    private bool upPressed = false;
    private bool downPressed = false;
    private bool isJumping = false;
    private bool autoJalan = false;
    private bool isClimbing = false;
    private int ladderCount = 0;

    private enum MovementState { idle, walk, jump, run, death }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        footstepAudio = GetComponent<AudioSource>();
        playerController = new PlayerController();
    }

    private void OnEnable()
    {
        playerController.Enable();
        playerController.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerController.Movement.Move.canceled += ctx => moveInput = Vector2.zero;
        playerController.Movement.Jump.performed += ctx => Jump();
    }

    private void OnDisable()
    {
        playerController.Disable();
    }

    private void Update()
    {
        if (Application.isMobilePlatform)
        {
            moveInput = new Vector2(mobileInputX, 0f);
            mobileInputY = isClimbing ? (upPressed ? 1f : (downPressed ? -1f : 0f)) : 0f;
        }
    }

    private void FixedUpdate()
    {
        CheckSlope();
        Vector2 velocity;

        if (autoJalan)
        {
            velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        else if (isClimbing)
        {
            rb.gravityScale = 0f;
            float vertical = Application.isMobilePlatform ? mobileInputY : moveInput.y;
            velocity = new Vector2(rb.velocity.x, vertical * moveSpeed);
        }
        else
        {
            rb.gravityScale = 1f;

            if (onSlope && isGrounded() && Mathf.Approximately(moveInput.x + mobileInputX, 0f))
            {
                velocity = slopeNormalPerp * -1f * (slopeAngle / 45f) * slopeSlidingSpeed;
            }
            else
            {
                float horizontal = moveInput.x + mobileInputX;
                velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
            }
        }

        rb.velocity = velocity;

        UpdateAnimation();
        HandleFootstepSound();

        if (isGrounded() && Mathf.Abs(rb.velocity.y) < 0.01f)
            isJumping = false;
    }

    private void UpdateAnimation()
    {
        MovementState state;
        float horizontal = moveInput.x != 0 ? moveInput.x : mobileInputX;

        if (rb.velocity.y > 0.1f)
            state = MovementState.jump;
        else if (Mathf.Abs(horizontal) > 0.1f)
            state = MovementState.walk;
        else
            state = MovementState.idle;

        sprite.flipX = horizontal switch
        {
            > 0f => false,
            < 0f => true,
            _ => sprite.flipX
        };

        anim.SetInteger("state", (int)state);
    }

    private bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private void Jump()
    {
        if (isGrounded() && !isClimbing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
        }
    }

    private void CheckSlope()
    {
        Vector2 origin = new Vector2(transform.position.x, coll.bounds.center.y);
        float distance = coll.bounds.extents.y + 0.1f;

        slopeHit = Physics2D.Raycast(origin, Vector2.down, distance, jumpableGround);

        if (slopeHit)
        {
            Vector2 normal = slopeHit.normal;
            slopeAngle = Vector2.Angle(normal, Vector2.up);

            if (slopeAngle > 10f && slopeAngle < 80f)
            {
                slopeNormalPerp = Vector2.Perpendicular(normal).normalized;
                onSlope = true;
            }
            else
            {
                onSlope = false;
            }
        }
        else
        {
            onSlope = false;
        }
    }

    // ==== Mobile Input Methods ====

    public void MoveRight(bool isPressed) => mobileInputX = isPressed ? 1f : 0f;
    public void MoveLeft(bool isPressed) => mobileInputX = isPressed ? -1f : 0f;

    public void UpButtonPressed()
    {
        upPressed = true;

        if (!isClimbing && isGrounded())
        {
            Jump();
        }
    }

    public void UpButtonReleased()
    {
        upPressed = false;
    }

    public void MoveDown(bool isPressed)
    {
        downPressed = isPressed;
    }

    // ==== Death, Bounce, Finish ====

    public void Die()
    {
        rb.velocity = Vector2.zero;

        if (footstepAudio != null && footstepAudio.isPlaying)
            footstepAudio.Stop();

        if (deathSource != null)
            deathSource.Play();
        else
            Debug.LogWarning("Death AudioSource belum di-assign!");

        anim.SetInteger("state", (int)MovementState.death);
        this.enabled = false;
    }

    private void Bounce()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce * 1.7f);
        isJumping = true;

        if (bounceSource != null)
            bounceSource.Play();
    }

    private void HandleFootstepSound()
    {
        bool isMoving = Mathf.Abs(moveInput.x + mobileInputX) > 0.1f;
        bool grounded = isGrounded();

        if (isMoving && grounded && !footstepAudio.isPlaying)
            footstepAudio.Play();
        else if ((!isMoving || !grounded) && footstepAudio.isPlaying)
            footstepAudio.Stop();
    }

    // ==== Trigger Events ====

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Enemy":
            case "Water":
                Die();
                break;

            case "Ladder":
                ladderCount++;
                isClimbing = true;
                break;

            case "Finish":
                autoJalan = true;
                ShowFinishMenu();
                break;

            case "Bounce":
                Bounce();
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            ladderCount--;
            if (ladderCount <= 0)
            {
                isClimbing = false;
                rb.gravityScale = 1f;
            }
        }
    }

    private void ShowFinishMenu()
    {
        Debug.Log("Level Selesai! Menampilkan UI...");

        if (finishSource != null)
            finishSource.Play();

        if (finishMenuUI != null)
        {
            finishMenuUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
