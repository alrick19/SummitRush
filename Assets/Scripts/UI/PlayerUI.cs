using UnityEngine.UI;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image staminaFill;
    public CanvasGroup staminaGroup;
    public float fadeSpeed = 2f;

    private Player player;
    private float staminaLeft;
    private Color baseColor;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
        baseColor = staminaFill.color;
    }

    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
    }

    void Update()
    {
        if (player == null) return;
        staminaLeft = player.currentClimbTime / player.maxClimbTime;
        UpdateStaminaUI();
        FadeStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        staminaFill.fillAmount = Mathf.Clamp01(staminaLeft);
    }
    private void FadeStaminaUI()
    {
        bool shouldBeVisible = player.currentClimbTime < player.maxClimbTime;
        float targetAlpha = shouldBeVisible ? 1f : 0f;
        staminaGroup.alpha = Mathf.MoveTowards(staminaGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);

        if (staminaLeft < 0.5f)
        {
            float pulse = Mathf.PingPong(Time.time * 5f, 1f); // pulse speed is 5
            Color pulseColor = Color.Lerp(Color.black, Color.white, pulse);
            staminaFill.color = pulseColor;
        }
        else
        {
            staminaFill.color = baseColor;
        }

    }
}
