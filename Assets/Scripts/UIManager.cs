using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Top HUD")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;

    [Header("Level Progress Bar")]
    public Slider levelProgressSlider;
    public TextMeshProUGUI levelProgressLabel;

    [Header("Court Damage Bar")]
    public Slider courtDamageSlider;
    public Image courtDamageFill;
    public TextMeshProUGUI courtDamageLabel;

    [Header("Feedback")]
    public Image screenFlash;
    public TextMeshProUGUI alertText;
    public TextMeshProUGUI comboPopup;

    [Header("End Panels")]
    public TextMeshProUGUI gameOverReason;
    public TextMeshProUGUI finalScoreText;

    void Start()
    {
        if (screenFlash) screenFlash.color = new Color(1f, 0f, 0f, 0f);
        if (alertText) alertText.gameObject.SetActive(false);
        UpdateScore(0);
    }

    public void UpdateScore(int s)
    {
        if (scoreText) scoreText.text = "Score: " + s.ToString("D6");
    }

    public void UpdateLevelDisplay(int level, int total)
    {
        if (levelText) levelText.text = "Level " + level + " / " + total;
    }

    public void UpdateLevelProgress(int done, int needed)
    {
        if (levelProgressSlider) { levelProgressSlider.maxValue = needed; levelProgressSlider.value = done; }
        if (levelProgressLabel) levelProgressLabel.text = "Sorted: " + done + " / " + needed;
    }

    public void UpdateCourtDamage(int dmg, int max)
    {
        if (courtDamageSlider) { courtDamageSlider.maxValue = max; courtDamageSlider.value = dmg; }
        if (courtDamageLabel) courtDamageLabel.text = "Court Damage: " + dmg + " / " + max;
        if (courtDamageFill)
            courtDamageFill.color = Color.Lerp(
                new Color(0.3f, 0.9f, 0.4f),
                new Color(0.95f, 0.2f, 0.2f),
                (float)dmg / max);
    }

    public void FlashWrongBin() => ShowAlert("WRONG BIN!", false);

    public void ShowAlert(string msg, bool isDanger)
    {
        StopCoroutine("AlertRoutine");
        StartCoroutine(AlertRoutine(msg, isDanger));
    }

    public void FlashDanger()
    {
        StopCoroutine("DangerFlash");
        StartCoroutine("DangerFlash");
    }

    public void ShowGameOver(string reason, int finalScore)
    {
        if (gameOverReason) gameOverReason.text = reason;
        if (finalScoreText) finalScoreText.text = "Score: " + finalScore.ToString("D6");
    }

    IEnumerator AlertRoutine(string msg, bool danger)
    {
        if (!alertText) yield break;
        alertText.color = danger ? new Color(1f, 0.3f, 0.3f) : new Color(1f, 0.85f, 0.2f);
        alertText.text = msg;
        alertText.gameObject.SetActive(true);
        yield return new WaitForSeconds(danger ? 1.5f : 0.8f);
        alertText.gameObject.SetActive(false);
    }

    IEnumerator DangerFlash()
    {
        if (!screenFlash) yield break;
        screenFlash.color = new Color(1f, 0f, 0f, 0.35f);
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            screenFlash.color = new Color(1f, 0f, 0f, Mathf.Lerp(0.35f, 0f, t / 0.4f));
            yield return null;
        }
        screenFlash.color = new Color(1f, 0f, 0f, 0f);
    }
}
