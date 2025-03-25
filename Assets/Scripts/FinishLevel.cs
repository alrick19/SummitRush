using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.CanFinishLevel())
        {
            GameManager.Instance.CompleteLevel();
        }
    }
}