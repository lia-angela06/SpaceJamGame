using System.Collections.Generic;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject rockPrefab;
    public GameObject metalPrefab;
    public GameObject crystalPrefab;
    public GameObject junkPrefab;

    [Header("Spawn Zone")]
    public float spawnY    =  6f;
    public float spawnXMin = -7f;
    public float spawnXMax =  7f;

    [Header("Difficulty — controlled by LevelManager")]
    public float initialInterval  = 2.2f;
    public float minimumInterval  = 1.0f;
    public float intervalDecay    = 0.04f;
    public float initialFallSpeed = 2.0f;
    public float maxFallSpeed     = 3.5f;

    private float currentInterval;
    private float timer;
    private int   spawnCount;
    private List<GameObject> prefabs = new List<GameObject>();

    void Start()  => ResetSpawner();

    void Update()
    {
        if (!GameManager.gameActive || prefabs.Count == 0) return;
        timer -= Time.deltaTime;
        if (timer <= 0f) { SpawnDebris(); timer = currentInterval; }
    }

    public void ResetSpawner()
    {
        currentInterval = initialInterval;
        timer           = initialInterval;
        spawnCount      = 0;
        prefabs.Clear();
        if (rockPrefab)    prefabs.Add(rockPrefab);
        if (metalPrefab)   prefabs.Add(metalPrefab);
        if (crystalPrefab) prefabs.Add(crystalPrefab);
        if (junkPrefab)    prefabs.Add(junkPrefab);
    }

    void SpawnDebris()
    {
        var prefab = prefabs[Random.Range(0, prefabs.Count)];
        float x    = Random.Range(spawnXMin, spawnXMax);
        var obj    = Instantiate(prefab, new Vector3(x, spawnY, 0f),
                         Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        var d = obj.GetComponent<DebrisObject>();
        if (d != null)
        {
            float t  = Mathf.Clamp01((float)spawnCount / 40f);
            d.fallSpeed = Mathf.Lerp(initialFallSpeed, maxFallSpeed, t);
        }

        spawnCount++;
        currentInterval = Mathf.Max(minimumInterval, currentInterval - intervalDecay);
    }

    public void StopSpawning() => enabled = false;
}
