using UnityEngine;

public class BurungGerak : MonoBehaviour
{
    public float kecepatan = 2f;
    public float jarakGerak = 3f;

    private Vector3 posisiAwal;
    private bool keKanan;
    private SpriteRenderer sr;

    void Start()
    {
        posisiAwal = transform.position;
        sr = GetComponent<SpriteRenderer>();

        // Arah awal ditentukan secara acak
        keKanan = Random.value > 0.5f;

        // Flip sprite kalau mulai ke kiri
        sr.flipX = !keKanan;
    }

    void Update()
    {
        if (keKanan)
        {
            transform.Translate(Vector2.right * kecepatan * Time.deltaTime);

            if (transform.position.x >= posisiAwal.x + jarakGerak)
            {
                keKanan = false;
                sr.flipX = true;
            }
        }
        else
        {
            transform.Translate(Vector2.left * kecepatan * Time.deltaTime);

            if (transform.position.x <= posisiAwal.x - jarakGerak)
            {
                keKanan = true;
                sr.flipX = false;
            }
        }
    }
}
