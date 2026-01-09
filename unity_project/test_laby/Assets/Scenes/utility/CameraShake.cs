using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    // Update is called once per frame
    public void Shake(float duration, float magnitude)
    {
       StartCoroutine(ProcessShake(duration, magnitude));
        
    }
    private IEnumerator ProcessShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Erzeugt eine zufällige Position in einem kleinen Kreis
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null; // Warte auf den nächsten Frame
        }

        transform.localPosition = originalPos; // Kamera wieder zurücksetzen
    }
}
    

