using UnityEngine;
using TMPro; // Wichtig für TextMesh Pro

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private const float DISAPPEAR_TIMER_MAX = 1f; // Wie lange die Zahl sichtbar ist

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    // Diese Methode rufen wir auf, um die Zahl zu initialisieren
    public void Setup(float damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        disappearTimer = DISAPPEAR_TIMER_MAX;

        // Zufällige Bewegung für mehr Dynamik
        moveVector = new Vector3(0.7f, 1) * 20f; 
        // Optional: Zufällige X-Richtung, damit Zahlen nicht stur geradeaus fliegen
        moveVector.x += Random.Range(-5f, 5f);
    }

    private void Update()
    {
        // 1. Bewegung nach oben
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime; // Verlangsamt die Bewegung sanft

        // 2. Countdown zum Verschwinden
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Startet das Ausblenden (Fading)
            float fadeSpeed = 3f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            // Wenn komplett unsichtbar -> Zerstören
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}