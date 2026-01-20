using UnityEngine;
using System.Collections;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs (5 total)")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Positions (8 total)")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public float spawnInterval = 2f;
    public int enemiesPerWave = 20;
    public float timeBetweenWaves = 5f;

    private bool isSpawning = false;


    void Start()
    {
        if (GameState.Instance == null)
        {
            Debug.LogError("GameState missing!");
            return;
        }

        GameState.Instance.OnCycleEnded += OnCycleEnded;
        Debug.Log("Enemy_Manager subscribed to OnCycleEnded");
    }

    void OnDestroy()
    {
        if (GameState.Instance != null)
            GameState.Instance.OnCycleEnded -= OnCycleEnded;
    }


    
    void OnCycleEnded()
    {
        StartCoroutine(SpawnWave());
    }

    

    IEnumerator SpawnWaves()
    {
        while (true) // infinite waves
        {
            yield return StartCoroutine(SpawnWave());
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemyWaveSpawner: Missing prefabs or spawn points!");
            return;
        }

        GameObject enemyPrefab =
            enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Transform spawnPoint =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}

