using UnityEngine;

public class KillDoppelTrigger : MonoBehaviour
{
    private bool triggered = false;
    private GameObject doppel;

    void Start()
    {
        doppel = GameObject.FindGameObjectWithTag("doppelganger");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !GameManager.Instance.CanFinishLevel()) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            if(doppel != null)
            {
                Destroy(doppel);
            }
        }
    }
}