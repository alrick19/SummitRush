using UnityEngine;

public class IcicleTrigger : MonoBehaviour
{
    private Icicle parentIcicle;

    void Start()
    {
        parentIcicle = GetComponentInParent<Icicle>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentIcicle.TriggerFall();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.icicle);
        }
    }
}