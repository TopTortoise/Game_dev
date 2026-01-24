using UnityEngine;
using System.Collections;

public class EnemyWaveSpawner : MonoBehaviour
{
    public static EnemyWaveSpawner Instance;

    [Header("Enemy Prefabs (6 total)")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Positions (10 total)")]
    public Transform[] spawnPoints;

    public GameObject boss;

    [Header("Wave Settings")]
    public float spawnInterval = 2f;
    public int enemiesPerWave = 20;
    public float timeBetweenWaves = 5f;

    public int nrBosses;

    public float nrWaves = 5f;

    private bool isSpawning = false;


void Awake()
    {
        // If an instance already exists and it's not us → destroy this
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // We are the singleton instance
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void InitializeEnemyWave()
    {
        spawnInterval = GameState.Instance.SpawnInterval;
        enemiesPerWave = GameState.Instance.EnemiesPerWave;
        nrBosses = GameState.Instance.nrBosses;
        Debug.Log("Enemy Wave Started with: "+ spawnInterval + " " + enemiesPerWave + " " + nrBosses);
    }

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
        InitializeEnemyWave();
        StartCoroutine(SpawnWave());
    }

    

    /*IEnumerator SpawnWaves()
    {
        
        for(int i = 0; i < nrWaves; i++)
        {
            yield return StartCoroutine(SpawnWave());
            yield return new WaitForSeconds(timeBetweenWaves);
        }
        
    }*/

    IEnumerator SpawnWave()
    {
        isSpawning = true;

        for (int i = 0; i < nrBosses; i++)
        {
            SpawnBoss();
            yield return new WaitForSeconds(spawnInterval);
        }

        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
            Debug.Log("enemy  " + i + " out of " + enemiesPerWave + " by " + Instance);
        }
        

        isSpawning = false;
        GameState.Instance.StartNewCycle(GameState.Instance.DayDuration);
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
        Debug.Log("enemy spawned by " + Instance);
    }

    void SpawnBoss()
    {
        if (boss == null)
        {
            return;
        }

        GameObject enemyPrefab = boss;

        Transform spawnPoint =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("boss spawned by " + Instance);
    }
}

