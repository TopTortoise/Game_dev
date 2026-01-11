using UnityEngine;

public class DamagePopupGenerator : MonoBehaviour
{
    // Singleton Pattern für einfachen Zugriff
    public static DamagePopupGenerator current;
    
    [SerializeField] private Transform damagePopupPrefab;

    private void Awake()
    {
        current = this;
    }

    public void CreatePopup(Vector3 position, float damageAmount)
    {
        // Erzeugt das Popup an der Position
        Transform damagePopupTransform = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        
        // Holt das Skript und setzt den Schaden
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);
    }
}