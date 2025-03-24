using UnityEngine;
using DG.Tweening;

public class DashTrail : MonoBehaviour
{
    private Player player;
    private AnimationScript anim;
    private Transform dashTrailParent;

    [Header("Trail Effects")]
    public Color trailColor;
    public Color fadeColor;
    public float afterEffectInterval;
    public float fadeTime;
    public ParticleSystem dashParticle;

    void Awake()
    {
        dashTrailParent = transform;
    }

    public void FadeSprite(SpriteRenderer sr)
    {
        sr.material.DOKill();
        sr.material.DOColor(fadeColor, fadeTime);
    }

    public void ShowTrail()
    {
        player = FindFirstObjectByType<Player>();
        anim = FindFirstObjectByType<AnimationScript>();


        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < dashTrailParent.childCount; i++)
        {
            Transform afterImage = dashTrailParent.GetChild(i);
            if (afterImage == null)
            {
                continue;
            }

            afterImage.gameObject.SetActive(true);


            SpriteRenderer sr = afterImage.GetComponent<SpriteRenderer>();
            if (sr == null || sr.material == null)
            {
                continue;
            }


            seq.AppendCallback(() =>
{
    afterImage.position = player.transform.position;
    sr.flipX = anim.sprite.flipX;

    if (dashParticle != null)
    {
        ParticleSystem particle = Instantiate(dashParticle, afterImage.position, Quaternion.identity);
        particle.Play();

        // Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
    }
});
            seq.Append(sr.material.DOColor(trailColor, 0));
            seq.AppendCallback(() => FadeSprite(sr));
            seq.AppendInterval(afterEffectInterval);
        }
    }
}
