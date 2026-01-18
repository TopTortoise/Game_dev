using UnityEngine;
using UnityEngine.UI; // Wichtig für UI
using UnityEngine.SceneManagement; // Wichtig für Szenen-Check
using System.Collections;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Einstellungen")]
    public float channelTime = 3.0f; // Dauert 3 Sekunden
    public KeyCode teleportKey = KeyCode.T; // Taste zum Teleportieren
    public float combatRadius = 10f; // Wie nah dürfen Gegner sein?
    public LayerMask enemyLayer; // Damit wir wissen, was ein Gegner ist

    [Header("Visuals")]
    public Image channelBar; // Zieh hier dein UI Image rein
    public GameObject channelEffect; // (Optional) Partikel während des Channelns

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

        // Abbrechen, wenn Taste losgelassen wird (optional) 
        // oder wenn man sich bewegt (siehe Coroutine)
        if (Input.GetKeyUp(teleportKey) && isChanneling)
        {
            CancelChannel();
        }
    }

    private bool CanTeleport()
    {
        // 1. Check: Sind wir im LootRoom? (Dort darf man sich meistens nicht wegteleportieren)
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "SmallLootRoom" || sceneName == "LargeLootRoom")
        {
            Debug.Log("Teleport hier nicht möglich!");
            return false;
        }

        // 2. Check: Sind wir im Kampf? (Gegner in der Nähe)
        Collider2D enemyNearby = Physics2D.OverlapCircle(transform.position, combatRadius, enemyLayer);
        if (enemyNearby != null)
        {
            Debug.Log("Nicht im Kampf teleportieren! Gegner in der Nähe: " + enemyNearby.name);
            // Hier könntest du einen roten Text einblenden "Gegner zu nah!"
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

            // Update UI Bar
            if (channelBar != null)
            {
                channelBar.fillAmount = timer / channelTime;
            }

            // ABBRUCH: Wenn Spieler sich bewegt
            if (Vector3.Distance(transform.position, startPos) > 0.1f)
            {
                Debug.Log("Bewegung erkannt - Teleport abgebrochen!");
                CancelChannel();
                yield break; // Coroutine beenden
            }

            // ABBRUCH: Wenn plötzlich Gegner kommen (optional, falls gewünscht)
            if (Physics2D.OverlapCircle(transform.position, combatRadius, enemyLayer))
            {
                Debug.Log("Gegner aufgetaucht - Teleport abgebrochen!");
                CancelChannel();
                yield break;
            }

            yield return null; // Warte auf nächsten Frame
        }

        // WENN FERTIG:
        PerformTeleport();
    }

    private void CancelChannel()
    {
        if (currentChannelRoutine != null) StopCoroutine(currentChannelRoutine);
        
        isChanneling = false;
        
        // Reset UI
        if (channelBar != null) channelBar.fillAmount = 0f;
        if (channelEffect != null) channelEffect.SetActive(false);
    }

    private void PerformTeleport()
    {
        Debug.Log("Teleport erfolgreich!");
        CancelChannel(); // UI zurücksetzen

        // Hier rufen wir dein PlayerPersistence auf!
        if (PlayerPersistence.Instance != null)
        {
            PlayerPersistence.Instance.RestoreReturnPosition();
        }
    }
    
    // Nur zum Sehen des Radius im Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, combatRadius);
    }
}