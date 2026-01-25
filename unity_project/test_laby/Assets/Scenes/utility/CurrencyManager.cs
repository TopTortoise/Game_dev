using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    public int currentGold;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddGold(int amount) 
    {
        AudioManager.Instance.Play(AudioManager.SoundType.Coin);
        currentGold += amount;
        Debug.Log("Goldstand: " + currentGold);
    }

    public void SpendCoins(int amount)
    {
        AudioManager.Instance.Play(AudioManager.SoundType.Coin);
        currentGold -= amount;
        Debug.Log("Goldstand: " + currentGold);
    }

    public void ResetCoins()
    {
        
        currentGold = 0;
        Debug.Log("Goldstand: " + currentGold);
    }

    public int currentCoins()
    {
        return currentGold;
    }

}