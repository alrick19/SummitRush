using System;
using System.Net;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem runDust;

    private Animator anim;
    private Player playerMove;
    private Collision coll;
    public SpriteRenderer sprite;
    private float verticalVelocity;

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
        runParticle();
    }

    public void HandleSpriteFlip()
    {
        // prevent flipping mid-climb/grab
        if (playerMove.isGrabbing && coll.rightWalled || coll.leftWalled)
            return;

        if (playerMove.horizontalMove > 0.01)
        {
            sprite.flipX = true;
            flipRunParticle(true);


        }
        else if (playerMove.horizontalMove < -0.01)
        {
            sprite.flipX = false;
            flipRunParticle(false);


        }
    }

    private void runParticle()
    {
        bool isRunning = playerMove.horizontalMove != 0;
        if (coll.isGrounded && isRunning)
        {
            if (!runDust.isEmitting)
                runDust.Play();
        }
        else
        {
            if (runDust.isEmitting)
                runDust.Stop();
        }
    }

    public void SetTrigger(string trigger)
    {
        anim.SetTrigger(trigger);
    }

    private void flipRunParticle(bool lookingRight)
    {
        var particleDir = runDust.forceOverLifetime.x;
        if (lookingRight)
        {
            runDust.transform.localPosition = new Vector3(-0.85f, -0.75f, 0);
            runDust.transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        else
        {
            runDust.transform.localPosition = new Vector3(0.85f, -0.75f, 0);
            runDust.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }


}
