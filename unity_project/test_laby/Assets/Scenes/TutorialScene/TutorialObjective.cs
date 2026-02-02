using UnityEngine;

public class TutorialObjective : MonoBehaviour
{
    public TutorialManager.TutorialStep nextStepOnDeath;
    private bool hasTriggered = false; 

    private bool isQuitting = false;
    void OnApplicationQuit() { isQuitting = true; }

    
    void OnDestroy()
    {
        if (isQuitting) return;
        CompleteObjective();
    }

    
    public void CompleteObjective()
    {
        if (hasTriggered) return;
        
        hasTriggered = true;

        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.AdvanceStep(nextStepOnDeath);
        }
    }
}