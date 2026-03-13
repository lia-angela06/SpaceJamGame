using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Rules")]
    public int   maxLives          = 3;
    public int   wrongBinPenalties = 2;
    public int   maxCourtDamage    = 5;
    public static float courtFloorY = -1f;

    public static bool gameActive = false;

    [HideInInspector] public int courtDamage = 0;
    private int currentLives  = 0;
    private int wrongBinCount = 0;

    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    void Start()
    {
        currentLives = maxLives;
        gameActive   = true;
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel)      winPanel.SetActive(false);
        FindObjectOfType<UIManager>()?.UpdateLives(currentLives, maxLives);
    }

    public void DebrisHitCourt(DebrisObject debris)
    {
        if (!gameActive) return;
        courtDamage++;
        FindObjectOfType<UIManager>()?.UpdateCourtDamage(courtDamage, maxCourtDamage);
        StartCoroutine(ScreenShake(0.15f, 0.08f));
        if (courtDamage >= maxCourtDamage) TriggerGameOver("The court is destroyed!");
    }

    public void EnemyReachedCourt(EnemyCharacter e)
    {
        if (!gameActive) return;
        FindObjectOfType<UIManager>()?.ShowAlert("A MONSTAR REACHED THE COURT!", true);
        StartCoroutine(ScreenShake(0.25f, 0.14f));
        LoseLife();
    }

    public void WrongBin()
    {
        if (!gameActive) return;
        wrongBinCount++;
        if (wrongBinCount >= wrongBinPenalties) { wrongBinCount = 0; LoseLife(); }
        FindObjectOfType<UIManager>()?.FlashWrongBin();
    }

    void LoseLife()
    {
        currentLives--;
        var ui = FindObjectOfType<UIManager>();
        ui?.UpdateLives(currentLives, maxLives);
        ui?.FlashDanger();
        if (currentLives <= 0) TriggerGameOver("You ran out of lives!");
    }

    public void TriggerGameOver(string reason)
    {
        if (!gameActive) return;
        gameActive = false;
        StopAll();
        if (gameOverPanel) gameOverPanel.SetActive(true);
        FindObjectOfType<UIManager>()?.ShowGameOver(reason, FindObjectOfType<ScoreManager>()?.score ?? 0);
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
        FindObjectOfType<DebrisSpawner>()?.StopSpawning();
        var es = FindObjectOfType<EnemySpawner>();
        if (es) es.enabled = false;
        foreach (var d in FindObjectsOfType<DebrisObject>())   Destroy(d.gameObject);
        foreach (var e in FindObjectsOfType<EnemyCharacter>()) Destroy(e.gameObject);
    }

    public void RestartGame()  { gameActive = false; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void LoadNextLevel() { gameActive = false; int n = SceneManager.GetActiveScene().buildIndex + 1; SceneManager.LoadScene(n < SceneManager.sceneCountInBuildSettings ? n : 0); }

    public System.Collections.IEnumerator ScreenShake(float dur, float mag)
    {
        var cam = Camera.main; var orig = cam.transform.position; float t = 0f;
        while (t < dur) { cam.transform.position = orig + (Vector3)(Random.insideUnitCircle * mag); t += Time.deltaTime; yield return null; }
        cam.transform.position = orig;
    }
}
