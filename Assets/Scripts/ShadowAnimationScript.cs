using UnityEngine;

public class ShadowAnimationScript : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sprite;
    private Vector2 previousPosition;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        previousPosition = transform.parent.position;
    }

    public void SetStates(bool isGrounded, bool isSliding, bool isJumping, bool isDashing)
    {
        Vector2 currentPosition = transform.parent.position;
        Vector2 velocity = (currentPosition - previousPosition) / Time.deltaTime;

        anim.SetFloat("horizontal", Mathf.Abs(velocity.x));
        anim.SetFloat("verticalVelocity", velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("canMove", true);

        // Triggers (only trigger when new, so they don’t spam)
        if (isJumping && !anim.GetCurrentAnimatorStateInfo(0).IsName("Jumping"))
        {
            anim.SetTrigger("Jumping");
        }

        if (isDashing && !anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
        {
            anim.SetTrigger("Dash");
        }

        // Flip
        if (velocity.x > 0.01f)
            sprite.flipX = true;
        else if (velocity.x < -0.01f)
            sprite.flipX = false;

        previousPosition = currentPosition;
    }
}