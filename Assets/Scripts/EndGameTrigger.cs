using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !GameManager.Instance.CanFinishLevel()) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            SceneHandler.Instance.LoadScene("MainMenu");
        }
    }
}