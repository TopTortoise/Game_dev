using UnityEngine;
using UnityEngine.UI; // Wichtig für UI
using UnityEngine.SceneManagement; // Wichtig für Szenen-Check
using System.Collections;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Einstellungen")]
    public float channelTime = 3.0f; // Dauert 3 Sekunden
    public KeyCode teleportKey = KeyCode.T; // Taste zum Teleportieren
    public float combatRadius = 10f; 
    public LayerMask enemyLayer; 

    [Header("Visuals")]
    public Image channelBar; 
    public GameObject channelEffect; 

    private bool isChanneling = false;
    private Coroutine currentChannelRoutine;

    void Update()
    {
        // Starten des Teleports
        if (Input.GetKeyDown(teleportKey))
        {
            if (CanTeleport())
            {
                currentChannelRoutine = StartCoroutine(ChannelTeleport());
            }
        }


        if (Input.GetKeyUp(teleportKey) && isChanneling)
        {
            CancelChannel();
        }
    }

    private bool CanTeleport()
    {
       
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "SmallLootRoom" || sceneName == "LargeLootRoom")
        {
            Debug.Log("Teleport hier nicht möglich!");
            return false;
        }

       
        Collider2D enemyNearby = Physics2D.OverlapCircle(transform.position, combatRadius, enemyLayer);
        if (enemyNearby != null)
        {
            Debug.Log("Nicht im Kampf teleportieren! Gegner in der Nähe: " + enemyNearby.name);
            
            return false;
        }

        return true;
    }

    private IEnumerator ChannelTeleport()
    {
        isChanneling = true;
        float timer = 0f;
        Vector3 startPos = transform.position;

        if (channelEffect != null) channelEffect.SetActive(true);

        Debug.Log("Teleport Channel gestartet...");

        while (timer < channelTime)
        {
            timer += Time.deltaTime;

           
            if (channelBar != null)
            {
                channelBar.fillAmount = timer / channelTime;
            }

           
            if (Vector3.Distance(transform.position, startPos) > 0.1f)
            {
                Debug.Log("Bewegung erkannt - Teleport abgebrochen!");
                CancelChannel();
                yield break; 
            }

            
            if (Physics2D.OverlapCircle(transform.position, combatRadius, enemyLayer))
            {
                Debug.Log("Gegner aufgetaucht - Teleport abgebrochen!");
                CancelChannel();
                yield break;
            }

            yield return null; 
        }

        
        PerformTeleport();
    }

    private void CancelChannel()
    {
        if (currentChannelRoutine != null) StopCoroutine(currentChannelRoutine);
        
        isChanneling = false;
        
        if (channelBar != null) channelBar.fillAmount = 0f;
        if (channelEffect != null) channelEffect.SetActive(false);
    }

    private void PerformTeleport()
    {
        Debug.Log("Teleport erfolgreich!");
        CancelChannel();

       
        if (PlayerPersistence.Instance != null)
        {
            PlayerPersistence.Instance.RestoreReturnPosition();
        }
    }
    
   
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, combatRadius);
    }
}