using UnityEngine;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Levels")]
    public int currentLevel = 1;
    public int totalLevels = 4;
    public int[] sortsToAdvance = { 1, 1, 1, 10 };

    [Header("Spawn Intervals Per Level")]
    public float[] spawnIntervals = { 2.2f, 2.5f, 2.8f, 4.0f };
    public float[] minimumIntervals = { 1.8f, 2.0f, 2.2f, 3.2f };

    [Header("Fall Speeds Per Level")]
    public float[] initialFallSpeeds = { 1.5f, 1.8f, 2.0f, 1.8f };
    public float[] maxFallSpeeds = { 2.0f, 2.2f, 2.5f, 2.2f };

    [Header("References")]
    public DebrisSpawner debrisSpawner;
    public UIManager uiManager;
    public TextMeshProUGUI levelBanner;

    private int sortsThisLevel = 0;

    void Start()
    {
        ApplyLevel(currentLevel);
    }

    public void OnCorrectSort()
    {
        sortsThisLevel++;
        int needed = sortsToAdvance[currentLevel - 1];
        uiManager?.UpdateLevelProgress(sortsThisLevel, needed);

        if (sortsThisLevel >= needed)
            StartCoroutine(AdvanceLevel());
    }

    IEnumerator AdvanceLevel()
    {
        if (currentLevel >= totalLevels)
        {
            FindFirstObjectByType<GameManager>()?.TriggerWin();
            yield break;
        }

        currentLevel++;
        sortsThisLevel = 0;

        if (debrisSpawner != null) debrisSpawner.enabled = false;

        foreach (var d in FindObjectsByType<DebrisObject>(FindObjectsSortMode.None)) Destroy(d.gameObject);

        if (levelBanner != null)
        {
            levelBanner.gameObject.SetActive(true);
            levelBanner.text = "LEVEL " + currentLevel;
            yield return new WaitForSeconds(2.2f);
            levelBanner.gameObject.SetActive(false);
        }
        else yield return new WaitForSeconds(1.5f);

        ApplyLevel(currentLevel);

        if (debrisSpawner != null) debrisSpawner.enabled = true;
    }

    void ApplyLevel(int level)
    {
        int i = level - 1;
        if (debrisSpawner != null)
        {
            debrisSpawner.initialInterval = spawnIntervals[i];
            debrisSpawner.minimumInterval = minimumIntervals[i];
            debrisSpawner.initialFallSpeed = initialFallSpeeds[i];
            debrisSpawner.maxFallSpeed = maxFallSpeeds[i];
            debrisSpawner.ResetSpawner();
        }
        uiManager?.UpdateLevelDisplay(level, totalLevels);
        uiManager?.UpdateLevelProgress(0, sortsToAdvance[i]);
    }
}
