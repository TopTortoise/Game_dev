using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public Image hpBarFill; // Das rote Bild der HP-Leiste

    void Update()
    {
        // Gold aktualisieren
        if (CurrencyManager.Instance != null)
        {
            goldText.text = CurrencyManager.Instance.currentGold.ToString();
        }

        // Hier kommt später die HP-Logik rein, sobald dein Player HP hat:
        // UpdateHealth(player.currentHealth, player.maxHealth);
    }
}