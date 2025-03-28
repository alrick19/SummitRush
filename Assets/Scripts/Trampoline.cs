using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float bounceForce = 35f;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.trampoline);
                animator.SetTrigger("Bounce");
                rb.linearVelocity = new Vector2(rb.linearVelocityX, bounceForce);
                player.isJumping = true;
            }
        }
    }
}
