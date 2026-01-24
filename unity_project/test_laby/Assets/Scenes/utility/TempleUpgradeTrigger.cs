using UnityEngine;

public class TempleUpgradeTrigger : MonoBehaviour
{
    public TempleUpgradeUI upgradeUI;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("attempt collision");

        
        {
            Debug.Log("Temple UI show");
            upgradeUI.Show();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
        {
            upgradeUI.Hide();
        }
    }
}

