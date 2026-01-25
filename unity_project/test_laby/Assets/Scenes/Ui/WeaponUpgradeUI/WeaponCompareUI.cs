using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text; 

public class WeaponCompareUI : MonoBehaviour
{
    public static WeaponCompareUI Instance;

    [Header("Panels")]
    public GameObject panel;

    [Header("Current Weapon")]
    public TextMeshProUGUI currentName;
    public TextMeshProUGUI currentDmg;
    public TextMeshProUGUI currentSpeed;
    public TextMeshProUGUI currentEffects; 
    public Image currentIcon;

    [Header("New Weapon")]
    public TextMeshProUGUI newName;
    public TextMeshProUGUI newDmg;
    public TextMeshProUGUI newSpeed;
    public TextMeshProUGUI newEffects;    
    public Image newIcon;

    private IWeapon pendingNewWeapon;
    private ghost playerRef;

    void Awake() { 
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this; panel.SetActive(false); 

        DontDestroyOnLoad(this.gameObject);
        }

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

        // Vergleic
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
            // 1. Name
            string cleanName = weapon.gameObject.name.Replace("(Clone)", "").Trim();
            tName.text = cleanName;

            // --- FIX START: VORSCHAU BERECHNEN ---
            float previewDmg = weapon.stats.damage;
            float previewSpeed = weapon.stats.attackspeed;

            // Wenn die Waffe NICHT ausgerüstet ist, müssen wir die Upgrades simulieren
            if (!weapon.is_equipped && weapon.upgrades != null)
            {
                foreach (var up in weapon.upgrades)
                {
                    // Wir prüfen, ob das Upgrade ein Statupgrade ist
                    if (up is Statupgrade statUp)
                    {
                        previewDmg += statUp.damageBonus;
                        previewSpeed += statUp.fireRateBonus;
                    }
                }
            }
            // --- FIX ENDE ---

            tDmg.text = previewDmg.ToString("F1");
            tSpeed.text = previewSpeed.ToString("F2");
            
            // 3. Effekte & Upgrades Text-Liste
            StringBuilder sb = new StringBuilder();

            if (weapon.upgrades != null && weapon.upgrades.Count > 0)
            {
                foreach (var up in weapon.upgrades)
                {
                    sb.AppendLine("↑ " + up.GetDescription());
                }
            }

            if (weapon.effects != null && weapon.effects.Count > 0)
            {
                foreach (var effect in weapon.effects)
                {
                    sb.AppendLine("• " + effect.GetDescription());
                }
            }

            if (sb.Length == 0) sb.Append("No Mods");
            tEffects.text = sb.ToString();

            // 4. Bild
            SpriteRenderer sr = weapon.GetComponent<SpriteRenderer>();
            if (sr == null) sr = weapon.GetComponentInChildren<SpriteRenderer>();

            if (sr != null && sr.sprite != null)
            {
                uiIcon.sprite = sr.sprite;
                uiIcon.enabled = true;
                uiIcon.color = Color.white; 
                uiIcon.preserveAspect = true;
            }
            else
            {
                uiIcon.enabled = false;
            }
        }
        else
        {
            // Empty State
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