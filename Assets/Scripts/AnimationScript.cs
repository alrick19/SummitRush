using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem runDust;

    private Animator anim;
    private Player playerMove;
    private Collision coll;
    private SpriteRenderer sprite;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMove = GetComponentInParent<Player>();
        coll = GetComponentInParent<Collision>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        anim.SetBool("isGrounded", coll.isGrounded);
        anim.SetBool("isWalled", coll.isWalled);
        anim.SetBool("rightWalled", coll.rightWalled);
        anim.SetBool("isGrabbing", playerMove.isGrabbing);
        anim.SetBool("isDashing", playerMove.isDashing);
        anim.SetBool("isSliding", playerMove.isSliding);
        anim.SetFloat("horizontalMove", Mathf.Abs(playerMove.horizontalMove));
        HandleSpriteFlip();
        runParticle();
    }

    public void HandleSpriteFlip()
    {
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
