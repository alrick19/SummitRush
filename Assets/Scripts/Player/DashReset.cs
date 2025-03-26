using UnityEngine;
using System.Collections;


public class DashReset : MonoBehaviour
{
    public float respawnTime = 2f;
    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.ResetDash();
                StartCoroutine(Collect());
            }
        }
    }

    private IEnumerator Collect()
    {
        // if (collectEffect != null)
        //     collectEffect.Play();

        sr.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        sr.enabled = true;
        col.enabled = true;
    }
}
