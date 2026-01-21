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

    public void StartNewCycle(int duration)
    {
        timeRemaining = duration;
        isCountingDown = true;
        warningStarted = false;

        OnCycleStarted?.Invoke();
    }

    public void EndCycle()
    {
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


