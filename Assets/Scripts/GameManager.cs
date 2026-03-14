using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Rules")]
    public int maxCourtDamage = 10;
    public static float courtFloorY = -1f;

    public static bool gameActive = false;

    [HideInInspector] public int courtDamage = 0;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    void Start()
    {
        gameActive = true;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
    }

    public void DebrisHitCourt(DebrisObject debris)
    {
        if (!gameActive) return;
        courtDamage++;
        FindFirstObjectByType<UIManager>()?.UpdateCourtDamage(courtDamage, maxCourtDamage);
        StartCoroutine(ScreenShake(0.15f, 0.08f));
        if (courtDamage >= maxCourtDamage) TriggerGameOver("The court is destroyed!");
    }

    public void WrongBin()
    {
        if (!gameActive) return;
        FindFirstObjectByType<UIManager>()?.FlashWrongBin();
        FindFirstObjectByType<UIManager>()?.FlashDanger();
    }

    public void TriggerGameOver(string reason)
    {
        if (!gameActive) return;
        gameActive = false;
        StopAll();
        if (gameOverPanel) gameOverPanel.SetActive(true);
        FindFirstObjectByType<UIManager>()?.ShowGameOver(reason, FindFirstObjectByType<ScoreManager>()?.score ?? 0);
    }

    public void TriggerWin()
    {
        if (!gameActive) return;
        gameActive = false;
        StopAll();
        if (winPanel) winPanel.SetActive(true);
    }

    void StopAll()
    {
        FindFirstObjectByType<DebrisSpawner>()?.StopSpawning();
        foreach (var d in FindObjectsByType<DebrisObject>(FindObjectsSortMode.None)) Destroy(d.gameObject);
    }

    public void RestartGame() { gameActive = false; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void LoadNextLevel() { gameActive = false; int n = SceneManager.GetActiveScene().buildIndex + 1; SceneManager.LoadScene(n < SceneManager.sceneCountInBuildSettings ? n : 0); }

    public System.Collections.IEnumerator ScreenShake(float dur, float mag)
    {
        var cam = Camera.main; var orig = cam.transform.position; float t = 0f;
        while (t < dur) { cam.transform.position = orig + (Vector3)(Random.insideUnitCircle * mag); t += Time.deltaTime; yield return null; }
        cam.transform.position = orig;
    }
}
