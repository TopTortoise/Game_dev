using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    public TextMeshProUGUI goldText;
    public Image hpBarFill;
    public TextMeshProUGUI introUI;
    public TextMeshProUGUI TorchText;


    void Awake()
    {
    

        // Singleton protection
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    
        Instance = this;
        DontDestroyOnLoad(gameObject);
    
        TipsUI();
    }

    void Update()
    {
        if (CurrencyManager.Instance != null)
        {
            goldText.text = CurrencyManager.Instance.currentGold.ToString();
        }

    }
    public void UpdateTorchUI(int currentTorches, int maxTorches)
    {
        if (TorchText != null)
        {
            TorchText.text = $"{currentTorches} / {maxTorches}";
        }
    }

    private void TipsUI()
    {
        //introUI.text = "Test"; This broke the HUD lol
    }
}
