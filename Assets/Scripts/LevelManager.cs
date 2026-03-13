using UnityEngine;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Levels")]
    public int currentLevel = 1;
    public int totalLevels  = 4;
    public int[] sortsToAdvance = { 1, 1, 1, 10 };

    [Header("Spawn Intervals Per Level")]
    public float[] spawnIntervals   = { 2.2f, 2.5f, 2.8f, 3.0f };
    public float[] minimumIntervals = { 1.8f, 2.0f, 2.2f, 2.4f };

    [Header("Fall Speeds Per Level")]
    public float[] initialFallSpeeds = { 1.5f, 1.8f, 2.0f, 2.2f };
    public float[] maxFallSpeeds     = { 2.0f, 2.2f, 2.5f, 2.8f };

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

        if (debrisSpawner != null) debrisSpawner.enabled = true;

        Debug.Log("Current level is: " + currentLevel);
        Debug.Log("EnemySpawner is null: " + (enemySpawner == null));

        if (currentLevel == 4 && enemySpawner != null)
        {
            enemySpawner.enabled = true;
            Debug.Log("ENEMY SPAWNER ACTIVATED");
        }
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
