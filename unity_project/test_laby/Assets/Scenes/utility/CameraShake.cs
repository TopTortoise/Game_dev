using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if(Instance == null) Instance = this;
    }

    // Update is called once per frame
    public void Shake()
    {
        StartCoroutine(ProcessShake(duration, magnitude));
        
    }
    private IEnumerator ProcessShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f)* magnitude;
            float y = Random.Range(-1f, 1f)* magnitude;

            transform.localPosition = new Vector3(x,y , originalPos.z);

            elapsed += Time.deltaTime;

            yield return null; 
        }
        transform.localPosition = originalPos;
    }
    
}
