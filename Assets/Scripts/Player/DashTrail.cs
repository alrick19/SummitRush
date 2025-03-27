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
        // sr.material.DOKill();
        // sr.material.DOColor(fadeColor, fadeTime);
        sr.DOKill();
        sr.DOColor(fadeColor, fadeTime);
    }

    public void ShowTrail()
    {
        player = FindFirstObjectByType<Player>();
        anim = FindFirstObjectByType<AnimationScript>();


        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < dashTrailParent.childCount; i++)
        {
            if (player == null)
                return;
            Transform afterImage = dashTrailParent.GetChild(i);
            if (afterImage == null)
            {
                continue;
            }



            SpriteRenderer sr = afterImage.GetComponent<SpriteRenderer>();
            if (sr == null || sr.material == null)
            {
                continue;
            }


            seq.AppendCallback(() =>
{
    if (player == null || anim == null || afterImage == null || sr == null)
        return;
    afterImage.gameObject.SetActive(true);

    afterImage.position = player.transform.position;
    sr.flipX = anim.sprite.flipX;

    if (dashParticle != null)
    {
        ParticleSystem particle = Instantiate(dashParticle, afterImage.position, Quaternion.identity);
        particle.Play();
    }
});
            // seq.Append(sr.material.DOColor(trailColor, 0));
            seq.Append(DOTween.To(() => sr.color, x => sr.color = x, trailColor, 0f));
            seq.AppendCallback(() => FadeSprite(sr));
            seq.AppendInterval(afterEffectInterval);
        }
    }
}
