using UnityEngine;

public class TutorialZone : MonoBehaviour
{
  
    public TutorialManager.TutorialStep stepToTrigger;

    private void OnTriggerEnter2D(Collider2D other)
{
    if (other.GetComponent<ghost>() != null)
    {
        Debug.Log("Trigger betreten! Sende Step: " + stepToTrigger); 
        TutorialManager.Instance.AdvanceStep(stepToTrigger);
        Destroy(gameObject);
    }
}
}