using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class DashTrail : MonoBehaviour
{
    private Transform trailTarget;
    private BaseAnimationScript animationScript;

    [Header("Trail Effects")]
    public Color trailColor;
    public Color fadeColor;
    public float afterEffectInterval = 0.05f;
    public float fadeTime = 0.4f;
    public ParticleSystem dashParticle;

    [Header("Afterimage Setup")]
    public GameObject afterImagePrefab;
    public int poolSize = 5;

    private List<GameObject> pool = new List<GameObject>();
    private int poolIndex = 0;

    public void Initialize(Transform target, BaseAnimationScript animScript)
    {
        trailTarget = target;
        animationScript = animScript;

        if (pool.Count == 0)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject img = Instantiate(afterImagePrefab, transform);
                img.SetActive(false);
                pool.Add(img);
            }
        }
    }

    public void ShowTrail()
    {
        if (trailTarget == null || animationScript == null)
        {
            Debug.LogWarning("DashTrail: trailTarget or animationScript not assigned.");
            return;
        }

        StartCoroutine(SpawnSequentialTrail());
    }

    private IEnumerator SpawnSequentialTrail()
    {
        for (int i = 0; i < poolSize; i++)
        {
            if (this == null) yield break; // in case trail is destroyed during dash

            GameObject afterImage = pool[poolIndex];
            poolIndex = (poolIndex + 1) % pool.Count;

            if (afterImage == null) continue;

            SpriteRenderer sr = afterImage.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            afterImage.SetActive(true);
            afterImage.transform.position = trailTarget.position;
            afterImage.transform.rotation = trailTarget.rotation;
            afterImage.transform.localScale = trailTarget.localScale;
            afterImage.transform.SetParent(null); // Detach

            sr.sprite = animationScript.sprite.sprite;
            sr.flipX = animationScript.sprite.flipX;
            sr.color = trailColor;

            if (dashParticle != null)
            {
                ParticleSystem particle = Instantiate(dashParticle, trailTarget.position, Quaternion.identity);
                particle.Play();
            }

            sr.DOKill();
            sr.DOColor(fadeColor, fadeTime).OnComplete(() =>
            {
                if (afterImage != null && this != null)
                {
                    afterImage.SetActive(false);
                    afterImage.transform.SetParent(transform);
                }
            });

            yield return new WaitForSeconds(afterEffectInterval);
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(this); // Clean up any active tweens
    }
}