using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class RollCredits : MonoBehaviour
{
    public GameObject creditsPanel;
    public RectTransform creditsContent;
    public float scrollSpeed = 50f;
    public float delayBeforeScroll = 1f;

    void Start()
    {
        creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        creditsPanel.SetActive(true);
        creditsContent.anchoredPosition = new Vector2(0, -Screen.height);
        StartCoroutine(ScrollCredits());
    }

    private IEnumerator ScrollCredits()
    {
        yield return new WaitForSeconds(delayBeforeScroll);

        while (creditsContent.anchoredPosition.y < creditsContent.sizeDelta.y + 2 * Screen.height)
        {
            creditsContent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null;
        }

        
        yield return new WaitForSeconds(1f);
        creditsPanel.SetActive(false);
        SceneManager.LoadScene("MainMenu");

    }
}