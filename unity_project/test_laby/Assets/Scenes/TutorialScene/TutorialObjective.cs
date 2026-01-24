using UnityEngine;

public class TutorialObjective : MonoBehaviour
{
    public TutorialManager.TutorialStep nextStepOnDeath;
    private bool hasTriggered = false; 

    private bool isQuitting = false;
    void OnApplicationQuit() { isQuitting = true; }

    // Diese Methode wird vom Enemy automatisch beim Zerstören aufgerufen
    void OnDestroy()
    {
        if (isQuitting) return;
        CompleteObjective();
    }

    // NEU: Diese Methode können wir von der Vase aus manuell aufrufen
    public void CompleteObjective()
    {
        if (hasTriggered) return; // Verhindert doppeltes Auslösen
        
        hasTriggered = true;

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.AdvanceStep(nextStepOnDeath);
        }
    }
}