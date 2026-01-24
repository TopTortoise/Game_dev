using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    public float templeHealth = 100;
    // ---- Clock state ----
    public int timeRemaining;
    public bool isCountingDown;
    public bool warningStarted;

    public bool enemyWaveActive;

    // ---- Progression events ----
    public event Action OnCycleEnded;
    public event Action OnCycleStarted;

    public event Action OnClockPaused;
    public event Action OnClockResumed;


    // ---- Game Stats (For Death Page)
    public int nrWavesDefeated;
    public int nrBossesDefeated;
    public int nrEnemiesDefeated;


    // ---- Enemy Wave State
    public float SpawnInterval = 2.5f;
    public int EnemiesPerWave = 10;

    public int nrBosses = 0;

    // ---- Day Length
    public int DayDuration = 300;

    //---- to keep tabs on entered lootrooms

    void UpdateEnemyWaveDifficulty()
    {
        nrBosses += 1;
        SpawnInterval -= UnityEngine.Random.Range(0.05f, 0.1f);;
        EnemiesPerWave += UnityEngine.Random.Range(2, 5);;
    }

    public void SetTempleHealth(float amount)
    {
        templeHealth = amount;
    }
    
    void UpdateDayDuration()
    {
        DayDuration += 60;
    }

    //for GameOverManager
    void ResetGameState()
    {
        SpawnInterval = 2.2f;
        EnemiesPerWave = 10;
        DayDuration = 300;
        nrWavesDefeated = 0;
        StartNewCycle(DayDuration);
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        nrBossesDefeated = 0;
        nrWavesDefeated = -1; //start with -1 (first day starts with 0)
    }

    public void StartNewCycle(int duration)
    {
        enemyWaveActive = false;
        AudioManager.Instance.ChangeMusic(AudioManager.SoundType.Music_Day);
        UpdateEnemyWaveDifficulty(); 
        nrWavesDefeated++;
        timeRemaining = duration;
        isCountingDown = true;
        warningStarted = false;

        OnCycleStarted?.Invoke();
    }

    public void EndCycle()
    {
        AudioManager.Instance.ChangeMusic(AudioManager.SoundType.Music_Defend_The_Temple);
        UpdateDayDuration();
        isCountingDown = false;
        enemyWaveActive = true;
        OnCycleEnded?.Invoke();
        Debug.Log("OnCycleEnded invoked");
    }

     public void PauseClock()
    {
        
        OnClockPaused?.Invoke();
    }

    public void ResumeClock()
    {
        
        OnClockResumed?.Invoke();
    }
}




/*using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    // ---- Clock state ----
    public int timeRemaining;
    public bool isCountingDown;
    public bool warningStarted;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}*/


