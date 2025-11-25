using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{   
    public int duration = 300; // 5 Minuten
    public int warningTime = 60; // Ab wann die Warnung startet (60 Sekunden = 1 Minute)
    public int timeRemaining;
    public bool isCountingDown = true;
    private bool warningStarted = false;
    
    public TMP_Text clockText;
    public ghost player;
    
    public void Start(){

        timeRemaining = duration;
        isCountingDown = true;
        Invoke("_tick", 1f);

    }

    public void StartTimer()
    {
        timeRemaining = duration;
        warningStarted = false;
        isCountingDown = true;
        Invoke("_tick", 1f);
    }

    private void _tick() 
    {
        timeRemaining--;
        
        // NUR wenn unter 1 Minute: Starte die Dunkelheit
        if (timeRemaining <= warningTime && player != null)
        {
            if (!warningStarted)
            {
                warningStarted = true;
                Debug.Log("WARNUNG: Weniger als 1 Minute! Es wird dunkel!");
            }
            
          
            float t = Mathf.Clamp01((float)timeRemaining / warningTime);
            player.ChangeSpotlight(t);  
            
            // Optional: Ändere die Textfarbe zur Warnung
            if (clockText != null)
            {
                clockText.color = Color.Lerp(Color.red, Color.white, t);
            }
        }
        
        // Update den Timer-Text
        if(timeRemaining > 0) 
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
            Debug.Log("ZEIT ABGELAUFEN! KAMPF BEGINNT!");
            
            
            if (player != null)
            {
                player.ChangeSpotlight(0f); // Komplett dunkel
            }
            // TODO: Spawne Gegner oder starte Kampf-Event
        }
    }
    
    public void ResetTimer()
    {
        isCountingDown = false;
        warningStarted = false;
        StartTimer();
    }
    
    // Gibt zurück ob die Warnung aktiv ist
    public bool IsWarningActive()
    {
        return timeRemaining <= warningTime && timeRemaining > 0;
    }
}