using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    private int combo = 0;
    private int totalSorted = 0;

    public TextMeshProUGUI comboPopup;
    private float hideTimer = 0f;

    void Update()
    {
        if (hideTimer > 0f)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f) comboPopup?.gameObject.SetActive(false);
        }
    }

    public void DebrisSorted(bool correct)
    {
        if (correct)
        {
            combo++;
            totalSorted++;
            score += 100 * (1 + combo / 5);
            ShowPopup();

            var lm = FindFirstObjectByType<LevelManager>();
            if (lm != null) lm.OnCorrectSort();
            else Debug.LogWarning("LevelManager not found!");
        }
        else
        {
            combo = 0;
        }
        FindFirstObjectByType<UIManager>()?.UpdateScore(score);
    }

    void ShowPopup()
    {
        if (comboPopup == null) return;
        string msg = combo >= 10 ? "TUNE UP! x" + combo
                   : combo >= 5 ? "ON FIRE! x" + combo
                   : combo >= 3 ? "NICE x" + combo
                   : "";
        if (msg == "") return;
        comboPopup.text = msg;
        comboPopup.gameObject.SetActive(true);
        hideTimer = 1.0f;
    }
}
