using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text; // Wichtig für StringBuilder

public class WeaponCompareUI : MonoBehaviour
{
    public static WeaponCompareUI Instance;

    [Header("Panels")]
    public GameObject panel;

    [Header("Current Weapon")]
    public TextMeshProUGUI currentName;
    public TextMeshProUGUI currentDmg;
    public TextMeshProUGUI currentSpeed;
    public TextMeshProUGUI currentEffects; // <--- NEU: Für die Effekt-Liste
    public Image currentIcon;

    [Header("New Weapon")]
    public TextMeshProUGUI newName;
    public TextMeshProUGUI newDmg;
    public TextMeshProUGUI newSpeed;
    public TextMeshProUGUI newEffects;    // <--- NEU: Für die Effekt-Liste
    public Image newIcon;

    private IWeapon pendingNewWeapon;
    private ghost playerRef;

    void Awake() { Instance = this; panel.SetActive(false); }

    public void ShowComparison(IWeapon newWeapon, ghost player)
    {
        pendingNewWeapon = newWeapon;
        playerRef = player;
        Time.timeScale = 0f;
        panel.SetActive(true);

        // Update Current
        UpdateWeaponUI(player.weapon, currentName, currentDmg, currentSpeed, currentEffects, currentIcon);

        // Update New
        UpdateWeaponUI(newWeapon, newName, newDmg, newSpeed, newEffects, newIcon);

        // Vergleich (wie vorher)
        if (player.weapon != null)
        {
            CompareStat(player.weapon.stats.damage, newWeapon.stats.damage, newDmg);
            CompareStat(player.weapon.stats.attackspeed, newWeapon.stats.attackspeed, newSpeed);
        }
    }

    void UpdateWeaponUI(IWeapon weapon, TMP_Text tName, TMP_Text tDmg, TMP_Text tSpeed, TMP_Text tEffects, Image uiIcon)
    {
        if (weapon != null)
        {
            // 1. FIX: Name bereinigen (Clone entfernen)
            string cleanName = weapon.gameObject.name.Replace("(Clone)", "").Trim();
            tName.text = cleanName;

            tDmg.text = weapon.stats.damage.ToString("F1");
            tSpeed.text = weapon.stats.attackspeed.ToString("F2");

            // 2. FIX: Effekte auflisten
            StringBuilder sb = new StringBuilder();
            
            // Wir gehen davon aus, dass IWeapon eine Liste "effects" hat
            // Falls deine Liste in "weapon.GetComponent<IWeapon>().effects" liegt, greif darauf zu.
            if (weapon.effects != null && weapon.effects.Count > 0)
            {
                foreach (var effect in weapon.effects)
                {
                    // Hier rufen wir die Methode aus Schritt 1 auf
                    sb.AppendLine("• " + effect.GetDescription());
                }
            }
            else
            {
                sb.Append("No Effects");
            }
            tEffects.text = sb.ToString();

            // 3. FIX: Bild laden
            SpriteRenderer sr = weapon.GetComponent<SpriteRenderer>();
            if (sr == null) sr = weapon.GetComponentInChildren<SpriteRenderer>();

            if (sr != null && sr.sprite != null)
            {
                uiIcon.sprite = sr.sprite;
                uiIcon.enabled = true;
                
                // WICHTIG: Farbe auf Weiß setzen, sonst ist es unsichtbar oder dunkel!
                uiIcon.color = Color.white; 
                uiIcon.preserveAspect = true;
            }
            else
            {
                uiIcon.enabled = false;
                // Debugging Hilfe:
                Debug.LogWarning($"Kein SpriteRenderer auf Waffe {cleanName} gefunden!");
            }
        }
        else
        {
            tName.text = "Empty";
            tDmg.text = "-";
            tSpeed.text = "-";
            tEffects.text = "";
            uiIcon.enabled = false;
        }
    }

    void CompareStat(float oldVal, float newVal, TMP_Text text)
    {
        if (newVal > oldVal) text.color = Color.green;
        else if (newVal < oldVal) text.color = Color.red;
        else text.color = Color.white;
    }
    
    
    public void OnClick_Equip() { playerRef.ConfirmSwapWeapon(pendingNewWeapon); CloseMenu(); }
    public void OnClick_Discard() { CloseMenu(); }
    void CloseMenu() { Time.timeScale = 1f; panel.SetActive(false); pendingNewWeapon = null; }
}