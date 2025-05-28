using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Untuk restart level

public class player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private bool isGrounded;
    private bool isDead = false;
    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private int ladderCount = 0;

    private bool isOnSlope = false;
    private Vector2 slopeNormal;
    public float slideSpeed = 5f; // Kecepatan meluncur di lereng
    private bool autoJalan = false;
    public GameObject finishMenuUI;


    Rigidbody2D body;
    SpriteRenderer sprite;
    Animator anim;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead)
        {
            anim.Play("Mati");
            return;
        }

        float moveInput = Input.GetAxisRaw("Horizontal");

        // Jika karakter di lereng, meluncur
        if (isOnSlope && isGrounded && body.velocity.y <= 0)
        {
            Vector2 slideDirection = new Vector2(slopeNormal.x, -slopeNormal.y);
            body.velocity = slideDirection * slideSpeed;
        }
        else
        {
            body.velocity = new Vector2(moveInput * speed, body.velocity.y);
        }

        if (moveInput != 0)
        {
            sprite.flipX = moveInput < 0;
        }

        anim.SetBool("Lari", moveInput != 0);
        anim.SetBool("Jump", !isGrounded);

        // Lompat
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            isGrounded = false;
        }

        // **Tangga**
        if (isClimbing)
        {
            float climbInput = Input.GetAxisRaw("Vertical");

            if (climbInput != 0)
            {
                body.velocity = new Vector2(body.velocity.x, climbInput * climbSpeed);
                body.gravityScale = 0;
            }
            else
            {
                body.velocity = new Vector2(body.velocity.x, 0.1f);
            }
        }
        else
        {
            body.gravityScale = 1;
        }

        if (autoJalan)
        {
            body.velocity = new Vector2(speed, body.velocity.y);
            sprite.flipX = false;
            anim.SetBool("Lari", true);
        }
    }

    void ShowFinishMenu()
    {
        if (finishMenuUI != null)
        {
            finishMenuUI.SetActive(true);
            Time.timeScale = 0f; // Optional: berhentikan waktu saat menu muncul
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            // Cek apakah tanah miring
            ContactPoint2D contact = collision.contacts[0];
            slopeNormal = contact.normal;
            float slopeAngle = Vector2.Angle(slopeNormal, Vector2.up);

            if (slopeAngle > 10f && slopeAngle < 80f)
            {
                isOnSlope = true;
            }
            else
            {
                isOnSlope = false;
            }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Mati();
        }
        else if (collision.gameObject.CompareTag("Ladder"))
        {
            ladderCount++;
            isClimbing = true;
        }
        else if (collision.gameObject.CompareTag("Water"))
        {
            Mati();
        }

        else if (collision.gameObject.CompareTag("Finish"))
        {
            autoJalan = true;
            ShowFinishMenu(); // Panggil fungsi untuk tampilkan UI
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
            }
        }
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
    