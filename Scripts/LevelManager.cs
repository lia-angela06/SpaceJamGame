using UnityEngine;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Levels")]
    public int currentLevel = 1;
    public int totalLevels  = 4;
    public int[] sortsToAdvance = { 10, 20, 30, 40 };

    [Header("Spawn Intervals Per Level")]
    public float[] spawnIntervals   = { 2.2f, 1.6f, 1.0f, 0.7f };
    public float[] minimumIntervals = { 1.0f, 0.7f, 0.45f, 0.3f };

    [Header("Fall Speeds Per Level")]
    public float[] initialFallSpeeds = { 2.0f, 2.8f, 3.6f, 4.2f };
    public float[] maxFallSpeeds     = { 3.5f, 4.5f, 5.5f, 6.5f };

    [Header("References")]
    public DebrisSpawner debrisSpawner;
    public EnemySpawner  enemySpawner;
    public UIManager     uiManager;
    public TextMeshProUGUI levelBanner;

    private int sortsThisLevel = 0;

    void Start()
    {
        if (enemySpawner != null) enemySpawner.enabled = false;
        ApplyLevel(currentLevel);
    }

    // Called by ScoreManager on every correct sort
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

        // Pause spawning during transition
        if (debrisSpawner != null) debrisSpawner.enabled = false;
        if (enemySpawner  != null) enemySpawner.enabled  = false;

        // Destroy in-flight debris between levels
        foreach (var d in FindObjectsByType<DebrisObject>(FindObjectsSortMode.None))  Destroy(d.gameObject);
        foreach (var e in FindObjectsByType<EnemyCharacter>(FindObjectsSortMode.None)) Destroy(e.gameObject);

        // Show banner
        if (levelBanner != null)
        {
            levelBanner.gameObject.SetActive(true);
            levelBanner.text = currentLevel == 4
                ? "FINAL LEVEL\nThe Monstars are here!"
                : "LEVEL " + currentLevel;
            yield return new WaitForSeconds(2.2f);
            levelBanner.gameObject.SetActive(false);
        }
        else yield return new WaitForSeconds(1.5f);

        ApplyLevel(currentLevel);

        if (debrisSpawner != null) debrisSpawner.enabled = true;
        if (currentLevel == 4 && enemySpawner != null) enemySpawner.enabled = true;
    }

    void ApplyLevel(int level)
    {
        int i = level - 1;
        if (debrisSpawner != null)
        {
            debrisSpawner.initialInterval  = spawnIntervals[i];
            debrisSpawner.minimumInterval  = minimumIntervals[i];
            debrisSpawner.initialFallSpeed = initialFallSpeeds[i];
            debrisSpawner.maxFallSpeed     = maxFallSpeeds[i];
            debrisSpawner.ResetSpawner();
        }
        uiManager?.UpdateLevelDisplay(level, totalLevels);
        uiManager?.UpdateLevelProgress(0, sortsToAdvance[i]);
    }
}
