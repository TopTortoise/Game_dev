using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement; // Wichtig für Scene-Events

public class SceneAutoSetup : MonoBehaviour
{
    // Wird aufgerufen, sobald das Objekt aktiv wird (also beim Spielstart)
    void OnEnable()
    {
        // Wir abonnieren das Event "SceneLoaded"
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Wird aufgerufen, wenn das Objekt zerstört wird (sauber bleiben!)
    void OnDisable()
    {
        // Abo kündigen, um Fehler zu vermeiden
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Diese Funktion wird AUTOMATISCH von Unity gefeuert, wenn eine Szene fertig ist
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndCreateEventSystem();
    }

    void CheckAndCreateEventSystem()
    {
        // Suche nach einem existierenden EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.Log("SceneAutoSetup: Kein EventSystem in dieser Szene gefunden. Erstelle Notfall-System...");
            
            GameObject eventSystemGO = new GameObject("Auto_EventSystem");
            
            // 1. Das Herzstück
            eventSystemGO.AddComponent<EventSystem>();
            
            // 2. Das Input Modul für das neue Input System
            eventSystemGO.AddComponent<InputSystemUIInputModule>();
        }
    }
}