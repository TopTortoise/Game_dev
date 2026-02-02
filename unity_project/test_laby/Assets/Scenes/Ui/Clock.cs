
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    public int duration = 300;
    public int warningTime = 60;

    public TMP_Text clockText;
    public ghost player;

    private Enemy_Manager em;

    public static Clock Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        em = GameObject.FindFirstObjectByType<Enemy_Manager>();

       
        if (GameState.Instance.timeRemaining <= 0)
        {
            InitializeTimer();
        }

        if (GameState.Instance == null)
        {
            Debug.LogError("GameState missing!");
            return;
        }

        GameState.Instance.OnCycleStarted += ResetTimer;
        GameState.Instance.OnClockPaused += PauseTicking;
        GameState.Instance.OnClockResumed += ResumeTicking;
        Debug.Log("Enemy_Manager subscribed to OnCycleEnded");


        ResumeTicking();
    }



   
    void InitializeTimer()
    {
        
        duration = GameState.Instance.timeRemaining ;
        GameState.Instance.isCountingDown = true;
        GameState.Instance.warningStarted = false;

        if (clockText != null)
            clockText.color = Color.white;

        if (player != null)
            player.ChangeSpotlight(1f);
    }

    void ResumeTicking()
    {
        CancelInvoke(nameof(_tick));

        if (GameState.Instance.isCountingDown) Invoke(nameof(_tick), 1f);
    }

  
    void _tick()
    {
        var gs = GameState.Instance;
        gs.timeRemaining--;

        if (gs.timeRemaining <= warningTime && !gs.warningStarted)
        {
            if (!gs.warningStarted)
                gs.warningStarted = true;
            AudioManager.Instance.ChangeMusic(AudioManager.SoundType.Music_Night_Coming);
        }
        // ---- Warning logic ----
        if (gs.timeRemaining <= warningTime && player != null)
        {
            

            float t = Mathf.Clamp01((float)gs.timeRemaining / warningTime);
            player.ChangeSpotlight(t);

            if (clockText != null)
                clockText.color = Color.Lerp(Color.red, Color.white, t);
        }

        // ---- Update UI ----
        if (gs.timeRemaining > 0)
        {
            int minutes = gs.timeRemaining / 60;
            int seconds = gs.timeRemaining % 60;

            if (clockText != null)
                clockText.text = $"{minutes:00}:{seconds:00}";

            Invoke(nameof(_tick), 1f);
        }
        else
        {
            OnTimerFinished();
        }
    }


    void OnDestroy()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.OnCycleStarted -= ResetTimer;
            GameState.Instance.OnClockPaused -= PauseTicking;
            GameState.Instance.OnClockResumed -= ResumeTicking;
        }
    }

    void OnTimerFinished()
    {
        if (player != null) player.ChangeSpotlight(1f);

        if (clockText != null) {
            clockText.color = Color.white;
            clockText.text = "Protect the Temple!";
        }
        GameState.Instance.EndCycle();
        
    }

    public void ResetTimer()
    {
        InitializeTimer();
        ResumeTicking();
    }

    void PauseTicking()
    {
        CancelInvoke(nameof(_tick));
    }


    public bool IsWarningActive()
    {
        var t = GameState.Instance.timeRemaining;
        return t <= warningTime && t > 0;
    }
}



