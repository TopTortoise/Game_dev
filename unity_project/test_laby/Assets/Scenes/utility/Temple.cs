
using UnityEngine;

public class Temple : MonoBehaviour, IKillable
{
    
    public float health = 20f;
    public float max_health = 20f;

    
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
    }
}

