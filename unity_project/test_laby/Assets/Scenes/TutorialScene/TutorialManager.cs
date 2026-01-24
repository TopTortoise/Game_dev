using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI Referenzen")]
    public TextMeshProUGUI instructionText;
    public float fadeSpeed = 1f;

    [Header("Spieler Referenz")]
    public ghost player; 
    public Transform playerSpawnPoint; 
    public Vector3 newSpawnPosition;

    [Header("Tutorial Gegner Settings")]
    public GameObject enemyPrefab;      // <-- Hier den Gegner reinziehen
    public Transform enemySpawnPoint;   // <-- Hier den Spawn-Ort reinziehen
    public int enemyCount = 5;

    // Die verschiedenen Phasen des Tutorials
    public enum TutorialStep
    {
        Movement,           // WASD benutzen
        GoToVase,           // Warte bis Spieler zur Vase läuft
        AttackVase,         // MBL drücken
        UpgradeGold,
        GoToEnemy,         // Achtung gegner
        KillEnemy,          // Gegner besiegen
        FindTemple,
            
        HealAtFountain,     // Angreifen zum Heilen 
        WeaponFound,
        GoToWell,
        SecondEnemy,
        WaveWarning,        // Countdown / Warnung
        PlaceTorches,       // Fackeln setzen (T/Rechtsklick) und aufheben (E)
        Teleport,
        Pause, 
        StartGame,
        TrapDash,           // Falle / Dash (Shift)
        Completed           // Fertig
    }

    public TutorialStep currentStep = TutorialStep.Movement;

    private bool isTextVisible = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (player == null) player = FindObjectOfType<ghost>();
        
        // Startet den ersten Schritt
        ShowText("Use W A S D to move.");
    }

    void Update()
    {
        
        switch (currentStep)
        {
            case TutorialStep.Movement:
                
                if (player.MoveAction.ReadValue<Vector2>() != Vector2.zero)
                {
                    AdvanceStep(TutorialStep.GoToVase); // Nächster Schritt: Gehe zur Vase
                }
                break;

            

            case TutorialStep.UpgradeGold:
                // Zeige Text für 4 Sekunden, dann beende Tutorial
                
                break;


            case TutorialStep.HealAtFountain:
                if (player.weapon != null && player.weapon.AttackAction.IsPressed())
                {
                    AdvanceStep(TutorialStep.WaveWarning);
                }
                break;

            case TutorialStep.PlaceTorches:
                // Prüft auf Fackel platzieren (Rechtsklick oder T (falls T belegt ist))
                // Oder Fackel aufheben (E)
                bool placed = player.PlaceTorchAction.WasPressedThisFrame();
                bool equip = player.EquipAction.WasPressedThisFrame(); // E

                if (placed || equip)
                {
                    AdvanceStep(TutorialStep.Teleport);
                }
                break;

            case TutorialStep.TrapDash:
                if (player.DashAction.WasPressedThisFrame())
                {
                   ShowText("Lets go!");
                }
                break;
            case TutorialStep.Teleport:
                bool teleport = player.Ret.WasPressedThisFrame();

                if (teleport)
                {
                    AdvanceStep(TutorialStep.Pause);
                }
                break; 
             
        }
    }

    // --- Hilfsfunktionen ---

   public void AdvanceStep(TutorialStep nextStep)
    {
        if (currentStep == nextStep) return; // Already in progress

        currentStep = nextStep;
        HideText(); // Hide old text

        // Logic for the NEW step
        switch (nextStep)
        {
            case TutorialStep.GoToVase:
               
                break;

            case TutorialStep.AttackVase:
                 ShowText("Use Left Click (LMB) to attack and destroy the vase!");
                break;

            case TutorialStep.GoToEnemy:
                ShowText("An enemy! Defeat it!");
                break;

            case TutorialStep.KillEnemy:
                ShowText("Well done!");
                break;

            case TutorialStep.WeaponFound:
                ShowText("Wow! You found a new weapon!!! You can equip it using [E].");
                break;
            case TutorialStep.FindTemple:
                ShowText("You can heal yourself at the well next to your temple");
                break;

            case TutorialStep.GoToWell:
                ShowText("Let's find this well.");
                break;
            
            case TutorialStep.SecondEnemy:
                ShowText("Ahh... I guess you can find anything in these vases.");
                break;

            case TutorialStep.HealAtFountain:
                ShowText("Hit the fountain to heal yourself faster!");
                break;

            case TutorialStep.WaveWarning:
                if (playerSpawnPoint != null)
                {
                    playerSpawnPoint.position = newSpawnPosition;
                    Debug.Log("Spawn point moved!");
                }
                StartCoroutine(WaveSequence());
                break;
                
            case TutorialStep.PlaceTorches:
                ShowText("Torches can help you defeat the waves. Use [Right Click] to place torches.\nUse [E] to pick them up.");
                break;
                
            case TutorialStep.Teleport:
                ShowText("If you ever want to go back to your Temple you can do this by pressing Q to Teleport back");
                break;

            case TutorialStep.TrapDash:
                ShowText("A trap! Use [SHIFT] to dash!");
                break;
            case TutorialStep.Pause:
                ShowText("You can always look up the controls in the Pausemenu! At the right corner of the screen");
               
                StartCoroutine(WaitAndAdvance(5f, TutorialStep.StartGame));
                break;

            case TutorialStep.StartGame:
                ShowText("Now you are Ready for your destiny!");
                
                StartCoroutine(EndTutorialSequence());
                break;
            case TutorialStep.UpgradeGold:
                ShowText("You can use your gold later to upgrade your temple!");
                break;
        }
    }

    // Special sequence for the wave
    IEnumerator WaveSequence()
    {
        ShowText("WARNING! Enemies want to destroy your temple!!!");

        if(AudioManager.Instance != null) 
            AudioManager.Instance.ChangeMusic(AudioManager.SoundType.Music_Defend_The_Temple);

        yield return StartCoroutine(SpawnEnemies());

        yield return new WaitForSeconds(1.5f);
        Debug.Log("AdvanceSetPlaceTorches");
        AdvanceStep(TutorialStep.PlaceTorches);
    }

    IEnumerator WaitAndAdvance(float seconds, TutorialStep nextStep)
    {
        yield return new WaitForSeconds(seconds);
        AdvanceStep(nextStep);
    }

    
    IEnumerator EndTutorialSequence()
    {
        
        yield return new WaitForSeconds(4f); 

        SceneManager.LoadScene("TitleScene"); 
    }
    IEnumerator SpawnEnemies()
    {
        if (enemyPrefab == null || enemySpawnPoint == null)
        {
            Debug.LogError("TutorialManager: Enemy Prefab oder SpawnPoint fehlt!");
            yield break;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            Vector3 spawnPos = enemySpawnPoint.position + offset;

            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

           
            yield return new WaitForSeconds(1.5f);
        }
    }
    // --- UI Steuerung ---
    private Coroutine currentFadeRoutine;
    public void ShowText(string text)
    {
        if (currentFadeRoutine != null) 
        {
            StopCoroutine(currentFadeRoutine);
            currentFadeRoutine = null;
        }
        
        instructionText.text = text;
        instructionText.alpha = 1f; 
        isTextVisible = true;
    }

   public void HideText()
    {
        if(instructionText != null)
        {
            
            if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);

            currentFadeRoutine = StartCoroutine(FadeOutText());
        }
    }

   IEnumerator FadeOutText()
    {
        float duration = 1f;
        float time = 0f;
        while(time < duration)
        {
            time += Time.deltaTime;
            instructionText.alpha = Mathf.Lerp(1f, 0f, time / duration);
            yield return null;
        }
        instructionText.alpha = 0f;
        currentFadeRoutine = null;
    }
}