using UnityEngine;
public class TempleInteract : MonoBehaviour
{
    public TempleUpgradeUI ui;
    bool inRange = false;

    void OnTriggerEnter2D(Collider2D other) { if(other.CompareTag("Player")) inRange = true; }
    void OnTriggerExit2D(Collider2D other) { if(other.CompareTag("Player")) inRange = false; }

    void Update()
    {
        // Drücke F um Shop zu öffnen
        if (inRange && Input.GetKeyDown(KeyCode.F))
        {
            ui.ToggleShop();
        }
    }
}