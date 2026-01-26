using UnityEngine;
using UnityEngine.UI;

public class TempleInteract : MonoBehaviour
{
    [Header("Referenzen")]
    public GameObject openShopPanel;
    public TempleUpgradeUI shopUI;   

    [Header("Einstellungen")]
    public string playerTag = "Player";

    void Start()
    {
        openShopPanel.SetActive(false);
    }
 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
           
            if (openShopPanel)
                openShopPanel.SetActive(true);
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {

            if (openShopPanel != null) 
                openShopPanel.SetActive(false);
            
           
            if(shopUI.gameObject.activeSelf) shopUI.ToggleShop();
        }
    }


    public void OnClickOpenShop()
    {
        shopUI.ToggleShop();
    }
}