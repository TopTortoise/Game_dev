using UnityEngine;
using TMPro; 

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private const float DISAPPEAR_TIMER_MAX = 1f;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(float damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        disappearTimer = DISAPPEAR_TIMER_MAX;

     
        moveVector = new Vector3(0.7f, 1) * 20f; 
        
        moveVector.x += Random.Range(-5f, 5f);
    }

    private void Update()
    {
        
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime; 

        
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
           
            float fadeSpeed = 3f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

           
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}