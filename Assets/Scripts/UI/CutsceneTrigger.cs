using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public string[] dialogueLines;
    public GameObject characterToShow; //cutscene doppelganger
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !GameManager.Instance.CanFinishLevel()) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            CutsceneManager.Instance.StartCutscene(dialogueLines, characterToShow);
        }
    }
}