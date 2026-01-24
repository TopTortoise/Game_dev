using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI Referenzen")]
    public TextMeshProUGUI instructionText;
    public float fadeSpeed = 1f;

    [Header("Spieler Referenz")]
    public ghost player; // ZIEHE DEN SPIELER HIER REIN

    // Die verschiedenen Phasen des Tutorials
    public enum TutorialStep
    {
        Movement,           // WASD benutzen
        GoToVase,           // Warte bis Spieler zur Vase läuft
        AttackVase,         // MBL drücken
        UpgradeGold,
        GoToEnemy,         // Achtung gegner
        KillEnemy,          // Gegner besiegen
            
        HealAtFountain,     // Angreifen zum Heilen 
        WaveWarning,        // Countdown / Warnung
        PlaceTorches,       // Fackeln setzen (T/Rechtsklick) und aufheben (E)
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
        ShowText("Benutze W A S D zum Bewegen");
    }

    void Update()
    {
        // Zustandsmaschine: Prüft je nach aktuellem Schritt, was zu tun ist
        switch (currentStep)
        {
            case TutorialStep.Movement:
                // Prüfen ob Spieler sich bewegt (Input System)
                if (player.MoveAction.ReadValue<Vector2>() != Vector2.zero)
                {
                    AdvanceStep(TutorialStep.GoToVase); // Nächster Schritt: Gehe zur Vase
                }
                break;

            

            case TutorialStep.UpgradeGold:
                // Zeige Text für 4 Sekunden, dann beende Tutorial
                AdvanceStep(TutorialStep.GoToEnemy);
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
                    AdvanceStep(TutorialStep.TrapDash);
                }
                break;

            case TutorialStep.TrapDash:
                if (player.DashAction.WasPressedThisFrame())
                {
                    AdvanceStep(TutorialStep.UpgradeGold);
                }
                break;
                
             
        }
    }

    // --- Hilfsfunktionen ---

    public void AdvanceStep(TutorialStep nextStep)
    {
        if (currentStep == nextStep) return; // Schon im Gange

        currentStep = nextStep;
        HideText(); // Alten Text ausblenden

        // Logik für den NEUEN Schritt
        switch (nextStep)
        {
            case TutorialStep.GoToVase:
               
                break;

            case TutorialStep.AttackVase:
                 ShowText("Nutze Linksklick (MBL), um anzugreifen und die Vase zu zerstören!");
                break;

            case TutorialStep.GoToEnemy:
                ShowText("Ein Gegner! Besiege ihn!");
                break;

            case TutorialStep.KillEnemy:
                
                break;


            case TutorialStep.HealAtFountain:
                ShowText("Schlage auf den Brunnen, um dich schneller zu heilen!");
                break;

            case TutorialStep.WaveWarning:
                StartCoroutine(WaveSequence());
                break;
                
            case TutorialStep.PlaceTorches:
                ShowText("Nutze [Rechtsklick], um Fackeln zu stellen.\nNutze [E], um Fackeln aufzuheben.");
                break;

            case TutorialStep.TrapDash:
                ShowText("Eine Falle! Nutze [SHIFT] zum Dashen!");
                break;
                
            case TutorialStep.UpgradeGold:
                ShowText("Du kannst dein Gold später verwenden um deinen Tempel zu verbessern!");
                break;
        }
    }

    // Spezielle Sequenz für die Welle
    IEnumerator WaveSequence()
    {
        ShowText("ACHTUNG! Gegner wollen deinen Tempel zerstören!");
        yield return new WaitForSeconds(4f);
        AdvanceStep(TutorialStep.PlaceTorches);
    }
    
    IEnumerator FinishTutorialDelay()
    {
        yield return new WaitForSeconds(5f);
        HideText();
        currentStep = TutorialStep.Completed;
    }


    // --- UI Steuerung ---

    public void ShowText(string text)
    {
        StopAllCoroutines(); 
        
        instructionText.text = text;
        instructionText.alpha = 1f; 
        isTextVisible = true;
    }

    public void HideText()
    {
        if(instructionText != null)
             StartCoroutine(FadeOutText());
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
    }
}