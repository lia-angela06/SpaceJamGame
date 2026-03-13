using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject enemyPrefab;

    [Header("Timing")]
    public float initialInterval = 4.0f;
    public float minimumInterval = 1.8f;
    public float intervalDecay   = 0.15f;

    [Header("Spawn Area")]
    public float spawnX    = 9f;
    public float spawnYMin = 2f;
    public float spawnYMax = 5f;

    [Header("Speed")]
    public float initialSpeed = 1.8f;
    public float maxSpeed     = 3.5f;

    private float currentInterval;
    private float timer;
    private int   spawnCount;

    void OnEnable()
    {
        currentInterval = initialInterval;
        timer      = 2.5f;
        spawnCount = 0;
    }

    void Update()
    {
        if (!GameManager.gameActive) return;
        timer -= Time.deltaTime;
        if (timer <= 0f) { SpawnEnemy(); timer = currentInterval; }
    }

    void SpawnEnemy()
    {
        if (!enemyPrefab) return;

        float side = (spawnCount % 2 == 0) ? -spawnX : spawnX;
        float y    = Random.Range(spawnYMin, spawnYMax);

        var obj = Instantiate(enemyPrefab, new Vector3(side, y, 0f), Quaternion.identity);

        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr && side > 0) sr.flipX = true;

        var ec = obj.GetComponent<EnemyCharacter>();
        if (ec != null)
        {
            ec.moveSpeed = Mathf.Lerp(initialSpeed, maxSpeed, Mathf.Clamp01((float)spawnCount / 15f));
            ec.targetY   = GameManager.courtFloorY;
        }

        spawnCount++;
        currentInterval = Mathf.Max(minimumInterval, currentInterval - intervalDecay);
    }
}
