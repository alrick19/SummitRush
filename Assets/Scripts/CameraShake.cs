using UnityEngine;
using DG.Tweening;
public class CameraShake : SingletonMonoBehavior<CameraShake>
{
    protected override void Awake()
    {
        base.Awake();
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
    }

    public static void Shake(float duration, float strength)
    {
        if (Instance != null)
        {
            Instance.OnShake(duration, strength);
        }
    }

    private void OnShake(float duration, float strength)
    {
        transform.DOShakePosition(duration, strength);
        transform.DOShakeRotation(duration, strength);
    }

}
