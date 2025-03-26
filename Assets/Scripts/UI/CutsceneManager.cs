using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CutsceneManager : SingletonMonoBehavior<CutsceneManager>
{
    public GameObject dialogueUI;
    public TextMeshProUGUI dialogueText;    
    public GameObject characterVisual; 

    private string[] lines;
    private int currentLine;
    private bool isCutsceneActive = false;

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
        //GameManager.Instance.PauseGame();
        InputManager.LockInput();
        isCutsceneActive = true;

        lines = dialogueLines;
        currentLine = 0;

        dialogueUI.SetActive(true);
        dialogueText.text = lines[currentLine];

        if (characterToShow != null)
        {
            characterToShow.SetActive(true);
            characterVisual = characterToShow;
        }
    }

    private void ShowNextLine()
    {
        currentLine++;

        if (currentLine < lines.Length)
        {
            dialogueText.text = lines[currentLine];
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

        //GameManager.Instance.ResumeGame();
        InputManager.UnlockInput();
    }

    public bool IsCutsceneActive => isCutsceneActive;
}