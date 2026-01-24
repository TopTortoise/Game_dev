
using UnityEngine;

public class Temple : MonoBehaviour, IKillable
{
    
    public float health = 100;
    public float max_health = 100f;

    
    public float regenInterval = 5f;   
    public float regenAmount = 10f;     
    private float regenTimer = 0f;

    
    public Sprite intactSprite;
    public Sprite destroyedSprite;

    private SpriteRenderer spriteRenderer;
    private Health hp;
    private bool destroyed = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hp = GetComponentInChildren<Health>();

        health = GameState.Instance.templeHealth;
        max_health = GameState.Instance.templeHealth;
        hp.set_max_hp(max_health);
        hp.set_hp(health);

        if (spriteRenderer != null && intactSprite != null)
            spriteRenderer.sprite = intactSprite;

        regenTimer = regenInterval;
    }

    void Update()
    {
        if (destroyed) return;

        
        if (hp.health >= hp.max_health) return;

        regenTimer -= Time.deltaTime;
        if (regenTimer <= 0f)
        {
            Debug.Log($"[Temple Regen] +{regenAmount} HP at time {Time.time:F1}");

            // Negative damage = healing
            hp.change_health(-regenAmount);

            regenTimer = regenInterval;
        }
    }

    void Start()
    {
        health = GameState.Instance.templeHealth;
        max_health = GameState.Instance.templeHealth;
        hp.set_max_hp(max_health);
        hp.set_hp(health);
    }

    public void UpgradeMaxHealth(float amount)
    {
        max_health += amount;
        health += amount;
        hp.set_max_hp(max_health);
        hp.set_hp(health);

        // Optional: fully heal on upgrade
        hp.set_hp(max_health);

        GameState.Instance.SetTempleHealth(max_health);
        GameState.Instance.SetCurrentTempleHealth(health);

        Debug.Log($"Temple upgraded! New max HP: {max_health}");
    }


    

    public void hit(float damage)
    {
        if (destroyed) return;

        hp.change_health(damage);
    }

    public void OnDeath()
    {
        if (destroyed) return;
        destroyed = true;

        if (spriteRenderer != null && destroyedSprite != null)
            spriteRenderer.sprite = destroyedSprite;

        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Debug.Log("Temple destroyed!");

        GameOverManager goManager = FindFirstObjectByType<GameOverManager>();
        if (goManager != null)
        {
            goManager.StartGameOver();
        }
        else
        {
            Debug.LogWarning("GameOverManager not Found!");
        }
    }
}

