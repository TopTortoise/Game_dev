using UnityEngine;
using System;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    // ---- Clock state ----
    public int timeRemaining;
    public bool isCountingDown;
    public bool warningStarted;

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
    public float SpawnInterval = 2.2f;
    public int EnemiesPerWave = 15;

    // ---- Day Length
    public int DayDuration = 300;


    void UpdateEnemyWaveDifficulty()
    {
        SpawnInterval -= 0.2f;
        EnemiesPerWave += 5;
    }
    
    void UpdateDayDuration()
    {
        DayDuration += 60;
    }

    //for GameOverManager
    void ResetGameState()
    {
        SpawnInterval = 2.2f;
        EnemiesPerWave = 15;
        DayDuration = 300;
        nrWavesDefeated = 0;
        StartNewCycle();
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
        UpdateEnemyWaveDifficulty();
        nrWavesDefeated++;
        timeRemaining = duration;
        isCountingDown = true;
        warningStarted = false;

        OnCycleStarted?.Invoke();
    }

    public void EndCycle()
    {
        UpdateDayDuration();
        isCountingDown = false;
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


