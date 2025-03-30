using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;    
    public GameObject characterVisual; 

    private string[] lines;
    private int currentLine;
    private bool isCutsceneActive = false;
    private Coroutine typeCoroutine;
    private bool isTyping = false;

    private void Update()
    {
        if (!isCutsceneActive) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNextLine();
        }
    }

    public void StartCutscene(string[] dialogueLines, GameObject characterToShow)
    {
        InputManager.LockInput();
        isCutsceneActive = true;

        lines = dialogueLines;
        currentLine = 0;

        dialogueUI.SetActive(true);

        typeCoroutine = StartCoroutine(TypeLine(lines[currentLine]));

        if (characterToShow != null)
        {
            characterToShow.SetActive(true);
            characterVisual = characterToShow;
        }
    }

    private void ShowNextLine()
    {
        if (isTyping)
        {
            // if player presses space mid-typing, finish the line instantly
            StopCoroutine(typeCoroutine);
            dialogueText.text = lines[currentLine];
            isTyping = false;
            AudioManager.Instance.StopTypingLoop();
            return;
        }

        currentLine++;

        if (currentLine < lines.Length)
        {
            typeCoroutine = StartCoroutine(TypeLine(lines[currentLine]));
        }
        else
        {
            EndCutscene();
        }
    }

    public void EndCutscene()
    {
        isCutsceneActive = false;
        dialogueUI.SetActive(false);

        if (characterVisual) characterVisual.SetActive(false);

        InputManager.UnlockInput();
    }

    public bool IsCutsceneActive => isCutsceneActive;

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        AudioManager.Instance.PlayTypingLoop(AudioManager.Instance.typing);
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f); // ttyping speed
        }
        AudioManager.Instance.StopTypingLoop();

        isTyping = false;
    }
}