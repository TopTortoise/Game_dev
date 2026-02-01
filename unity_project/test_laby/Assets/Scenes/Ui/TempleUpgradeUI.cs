using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TempleUpgradeUI : MonoBehaviour
{
    [Header("General UI")]
    public GameObject shopPanel;
    public Temple temple;

    [Header("Health Upgrade")]
    public TextMeshProUGUI hpLevelText;
    public TextMeshProUGUI hpCostText;
    public TextMeshProUGUI hpValueText;
    public Button hpButton;

    [Header("Damage Upgrade")]
    public TextMeshProUGUI dmgLevelText;
    public TextMeshProUGUI dmgCostText;
    public TextMeshProUGUI dmgValueText;
    public Button dmgButton;

    [Header("Cooldown Upgrade")]
    public TextMeshProUGUI cdLevelText;
    public TextMeshProUGUI cdCostText;
    public TextMeshProUGUI cdValueText;
    public Button cdButton;

    private bool isOpen = false;
    // damage +0.25 damge increase
    // health to 250 max lvl
    // cd -2 sec cd 
    void Start()
    {
        shopPanel.SetActive(false);
        if (temple == null) temple = FindObjectOfType<Temple>();
    }

    public void ToggleShop()
    {
        isOpen = !isOpen;
        shopPanel.SetActive(isOpen);

        UpdateUI();
        GameState.Instance.RequestPause(isOpen);
    }
void UpdateUI()
{
    int coins = CurrencyManager.Instance.currentGold;
    int maxLevel = 7; //level cap

    // --- HEALTH ---
    int hpLvl = GameState.Instance.levelHealth;
    int hpCost = temple.GetUpgradeCost(hpLvl);
    
    hpLevelText.text = $"Lvl {hpLvl}";
    hpCostText.text = hpLvl >= maxLevel ? "MAX" : $"{hpCost} G";
    hpValueText.text = $"{temple.max_health} HP";
    hpButton.interactable = (hpLvl < maxLevel) && (coins >= hpCost);

    // --- DAMAGE ---
    int dmgLvl = GameState.Instance.levelUltDamage;
    int dmgCost = temple.GetUpgradeCost(dmgLvl);

    dmgLevelText.text =  $"Lvl {dmgLvl}";
    dmgCostText.text = dmgLvl >= maxLevel ? "MAX" : $"{dmgCost} G";
    dmgValueText.text = $"{temple.damage:F2} Dmg"; // F2 for decimal precision
    dmgButton.interactable = (dmgLvl < maxLevel) && (coins >= dmgCost);

    // --- COOLDOWN ---
    int cdLvl = GameState.Instance.levelUltCooldown;
    int cdCost = temple.GetUpgradeCost(cdLvl);

    cdLevelText.text = $"Lvl {cdLvl}";
    cdCostText.text = cdLvl >= maxLevel ? "MAX" : $"{cdCost} G";
    cdValueText.text = $"{temple.attackInterval:F1}s CD";
    
    // Interactable if below level 6 AND enough coins AND above minCooldown
    cdButton.interactable = (cdLvl < maxLevel) && (coins >= cdCost) && (temple.attackInterval > temple.minCooldown);
}

    // --- BUTTON EVENTS ---

    public void OnBuyHealth()
    {
        int lvl = GameState.Instance.levelHealth;
        int cost = temple.GetUpgradeCost(lvl);

        if (CurrencyManager.Instance.currentGold >= cost)
        {
            CurrencyManager.Instance.SpendCoins(cost);
            GameState.Instance.levelHealth++;
            temple.RecalculateStats();
            //AudioManager.Instance.Play(AudioManager.SoundType.Upgrade); // Falls du einen Sound hast
            UpdateUI();
        }
    }

    public void OnBuyDamage()
    {
        int lvl = GameState.Instance.levelUltDamage;
        int cost = temple.GetUpgradeCost(lvl);

        if (CurrencyManager.Instance.currentGold >= cost)
        {
            CurrencyManager.Instance.SpendCoins(cost);
            GameState.Instance.levelUltDamage++;
            temple.RecalculateStats();
            UpdateUI();
        }
    }

    public void OnBuyCooldown()
    {
        int lvl = GameState.Instance.levelUltCooldown;
        int cost = temple.GetUpgradeCost(lvl);

        if (CurrencyManager.Instance.currentGold >= cost)
        {
            CurrencyManager.Instance.SpendCoins(cost);
            GameState.Instance.levelUltCooldown++;
            temple.RecalculateStats();
            UpdateUI();
        }
    }

    public void OnClose()
    {
        ToggleShop();
    }
}