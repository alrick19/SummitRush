using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AnimationScript : BaseAnimationScript
{
    private Animator anim;
    private Player playerMove;
    private Collision coll;
    private float verticalVelocity;
    [SerializeField] public Light2D dashLight;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMove = GetComponentInParent<Player>();
        coll = GetComponentInParent<Collision>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        verticalVelocity = playerMove.GetComponent<Rigidbody2D>().linearVelocity.y;
        anim.SetFloat("verticalVelocity", verticalVelocity);
        anim.SetFloat("horizontal", Mathf.Abs(playerMove.horizontalMove));
        anim.SetFloat("vertical", Mathf.Abs(playerMove.verticalMove));
        anim.SetBool("isGrabbing", playerMove.isGrabbing);
        anim.SetBool("isDashing", playerMove.isDashing);
        anim.SetBool("canMove", playerMove.canMove);
        anim.SetBool("isGrounded", coll.isGrounded);
        anim.SetBool("isSliding", playerMove.isSliding);
        HandleSpriteFlip();
        HandleDashLight();
        HandleLightFlip();

    }

    public void HandleSpriteFlip()
    {
        if (playerMove.isGrabbing && (coll.rightWalled || coll.leftWalled))
            return;

        if (playerMove.horizontalMove > 0.01f)
        {
            sprite.flipX = true;
        }
        else if (playerMove.horizontalMove < -0.01f)
        {
            sprite.flipX = false;
        }
    }

    private void HandleDashLight()
    {
        if (playerMove.hasDashed)
        {
            if (dashLight.enabled)
                dashLight.enabled = false;
        }
        else
        {
            if (!dashLight.enabled)
                dashLight.enabled = true;
        }
    }

    private void HandleLightFlip()
    {
        if (playerMove.isGrabbing && (coll.rightWalled || coll.leftWalled))
            return;

        Vector3 scale = dashLight.transform.localScale;

        if (playerMove.horizontalMove > 0.01f)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        else if (playerMove.horizontalMove < -0.01f)
        {
            scale.x = Mathf.Abs(scale.x);
        }

        dashLight.transform.localScale = scale;
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

}