using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    public int duration = 300;
    public int warningTime = 60;
    public int timeRemaining;
    public bool isCountingDown = true;
    private bool warningStarted = false;
    private Enemy_Manager em;
    public TMP_Text clockText;
    public ghost player;
    public static Clock Instance;

    public void Start()
    {
        Instance = this;
       
        StartTimer();
        em = GameObject.FindFirstObjectByType<Enemy_Manager>();
    }

    public void StartTimer()
    {
        CancelInvoke("_tick"); 
        timeRemaining = duration;
        warningStarted = false;
        isCountingDown = true;
        
      
        if (clockText != null) clockText.color = Color.white;
        if (player != null) player.ChangeSpotlight(1f);

        Invoke("_tick", 1f);
    }

    private void _tick()
    {
        timeRemaining--;

       
        if (timeRemaining <= warningTime && player != null)
        {
            if (!warningStarted)
            {
                warningStarted = true;
            }

            float t = Mathf.Clamp01((float)timeRemaining / warningTime);
            player.ChangeSpotlight(t);

          
            if (clockText != null)
            {
                clockText.color = Color.Lerp(Color.red, Color.white, t);
            }
        }

        
        if (timeRemaining > 0)
        {
            int minutes = timeRemaining / 60;
            int seconds = timeRemaining - minutes * 60;
            if (clockText != null)
            {
                clockText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            Invoke("_tick", 1f);
        }
        else
        {
           
            isCountingDown = false;

            if (player != null)
            {
               
                player.ChangeSpotlight(1f); 
                
                player.transform.position = player.spawn_pos;
                // StartCoroutine(em.spawnWave());
            }

            
            if (clockText != null)
            {
                clockText.color = Color.white;
            }
        }
    }

    public void ResetTimer()
    {
        StartTimer();
    }

    public bool IsWarningActive()
    {
        return timeRemaining <= warningTime && timeRemaining > 0;
    }
}
