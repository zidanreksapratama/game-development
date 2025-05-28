using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAutoMove : MonoBehaviour
{
    public float speed = 3f;
    public float jumpForce = 5f;
    public float jumpInterval = 2f;
    public float moveDistance = 5f;

    public float climbSpeed = 3f;
    public float slideSpeed = 5f;

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator anim;

    private bool isGrounded = false;
    private bool isDead = false;
    private bool isClimbing = false;
    private int ladderCount = 0;

    private bool isOnSlope = false;
    private Vector2 slopeNormal;

    private bool movingRight = true;
    private Vector2 startPos;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        startPos = transform.position;

        StartCoroutine(JumpRoutine());
    }

    void Update()
    {
        if (isDead)
        {
            anim.Play("Mati");
            return;
        }

        AutoMove();
        anim.SetBool("Lari", true);
        anim.SetBool("Jump", !isGrounded);
    }

    void AutoMove()
    {
        float direction = movingRight ? 1f : -1f;

        if (isOnSlope && isGrounded && body.velocity.y <= 0)
        {
            Vector2 slideDirection = new Vector2(slopeNormal.x, -slopeNormal.y);
            body.velocity = slideDirection * slideSpeed;
        }
        else
        {
            body.velocity = new Vector2(direction * speed, body.velocity.y);
        }

        sprite.flipX = direction < 0;

        // Ganti arah jika mencapai batas
        if (movingRight && transform.position.x >= startPos.x + moveDistance)
            movingRight = false;
        else if (!movingRight && transform.position.x <= startPos.x - moveDistance)
            movingRight = true;
    }

    IEnumerator JumpRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(jumpInterval);
            if (isGrounded && !isClimbing)
            {
                body.velocity = new Vector2(body.velocity.x, jumpForce);
                isGrounded = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            ContactPoint2D contact = collision.contacts[0];
            slopeNormal = contact.normal;
            float slopeAngle = Vector2.Angle(slopeNormal, Vector2.up);

            isOnSlope = (slopeAngle > 10f && slopeAngle < 80f);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            isOnSlope = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Water"))
        {
            Mati();
        }
        else if (collision.gameObject.CompareTag("Ladder"))
        {
            ladderCount++;
            isClimbing = true;
            body.gravityScale = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            ladderCount--;
            if (ladderCount <= 0)
            {
                isClimbing = false;
                ladderCount = 0;
                body.gravityScale = 1;
            }
        }
    }

    public void Mati()
    {
        isDead = true;
        anim.SetTrigger("Mati");
        anim.SetBool("Lari", false);
        anim.SetBool("Jump", false);

        body.velocity = Vector2.zero;
        body.isKinematic = true;
        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
