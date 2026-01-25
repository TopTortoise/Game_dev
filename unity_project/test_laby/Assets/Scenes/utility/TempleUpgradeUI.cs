using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TempleUpgradeUI : MonoBehaviour
{
    [Header("Setup")]
    public Temple temple;
    public Button upgradeButton;
    public TMP_Text priceText;

    [Header("Upgrade")]
    public int upgradeCost = 50;
    public float healthIncrease = 50f;

    private bool purchased = false;

    void Awake()
    {
        
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeButtonPressed);

        Hide();
    }

    void OnEnable()
    {
        RefreshUI();
    }

    void Update()
    {
        if (!gameObject.activeSelf || purchased)
            return;

        RefreshUI();
    }

    void RefreshUI()
    {
        bool canAfford = CurrencyManager.Instance.currentGold >= upgradeCost;

        // Visual feedback ONLY
        upgradeButton.interactable = canAfford;
        priceText.color = canAfford ? Color.green : Color.red;
        priceText.text = $"Upgrade Temple ({upgradeCost})";
    }

    public void Show()
    {
        gameObject.SetActive(true);
        RefreshUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void OnUpgradeButtonPressed()
    {
        Debug.Log("Upgrade button pressed");

        if (purchased)
            return;

        if (CurrencyManager.Instance.currentGold < upgradeCost)
        {
            Debug.Log("Not enough gold");
            return;
        }

        // ---- APPLY UPGRADE ----
        purchased = true;

        CurrencyManager.Instance.SpendCoins(upgradeCost);
        temple.UpgradeMaxHealth(healthIncrease);

        Debug.Log("Temple upgrade purchased!");

        priceText.text = "Temple upgraded!";
        priceText.color = Color.white;
    }
}
