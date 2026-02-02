using UnityEngine;

public class DamagePopupGenerator : MonoBehaviour
{
        public static DamagePopupGenerator current;
    
    [SerializeField] private Transform damagePopupPrefab;

    private void Awake()
    {
        current = this;
    }

    public void CreatePopup(Vector3 position, float damageAmount)
    {
       
        Transform damagePopupTransform = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        
       
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);
    }
}