using UnityEngine;
using System.Collections;

public class FallingBlock : MonoBehaviour, IResettableHazard
{
    private Rigidbody2D rb;
    private Vector2 startPos;
    private bool hasFallen = false;

    void Awake()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        RoomHazardManager room = GetComponentInParent<RoomHazardManager>();

        if (room != null)
        {
            room.Register(this);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !hasFallen)
        {
            StartCoroutine(FallSequence());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasFallen)
        {
            StartCoroutine(FallSequence());

        }
    }

    private IEnumerator FallSequence()
    {
        // yield return new WaitForSeconds(0.5f); // Wait before shake

        // // Shake animation (small position jitter)
        // float shakeDuration = 0.3f;
        // float shakeIntensity = 0.05f;
        // Vector3 originalPos = transform.position;

        // float elapsed = 0f;
        // while (elapsed < shakeDuration)
        // {
        //     float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
        //     float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
        //     transform.position = originalPos + new Vector3(offsetX, offsetY, 0);
        //     elapsed += Time.deltaTime;
        //     yield return null;
        // }

        // transform.position = originalPos;
        yield return new WaitForSeconds(0.5f);
        TriggerFall();
    }


    public void TriggerFall()
    {
        if (hasFallen) return;
        hasFallen = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.fallBlock);
    }

    public void ResetHazard()
    {
        hasFallen = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        transform.position = startPos;
        gameObject.SetActive(true);
    }
}
