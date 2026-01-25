using UnityEngine;
using UnityEngine.UI;

public class TempleInteract : MonoBehaviour
{
    [Header("Referenzen")]
    public GameObject openShopPanel; // Der Button im Canvas
    public TempleUpgradeUI shopUI;    // Dein Shop-Fenster Skript

    [Header("Einstellungen")]
    public string playerTag = "Player"; // Tag deines Spielers

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
            // Button verstecken
            if (openShopPanel != null) 
                openShopPanel.SetActive(false);
            
           
            if(shopUI.gameObject.activeSelf) shopUI.ToggleShop();
        }
    }

    // DIESE FUNKTION kommt auf den Button "OnClick"
    public void OnClickOpenShop()
    {
        // 1. Shop öffnen
        shopUI.ToggleShop();

       
    }
}