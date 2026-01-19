
using UnityEngine;

public class Temple : MonoBehaviour, IKillable
{
    // -------- Stats (same pattern as enemies) --------
    public float health = 20f;
    public float max_health = 20f;

    // -------- Regeneration --------
    public float regenInterval = 5f;   // seconds
    public float regenAmount = 1f;     // HP per tick
    private float regenTimer = 0f;

    // -------- Visuals --------
    public Sprite intactSprite;
    public Sprite destroyedSprite;

    private SpriteRenderer spriteRenderer;
    private Health hp;
    private bool destroyed = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hp = GetComponentInChildren<Health>();

        // SAME initialization pattern as Water_Slime
        hp.set_max_hp(max_health);
        hp.set_hp(health);

        if (spriteRenderer != null && intactSprite != null)
            spriteRenderer.sprite = intactSprite;

        regenTimer = regenInterval;
    }

    void Update()
    {
        if (destroyed) return;

        // Do not regen if already full HP
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

    // -------- IKillable --------

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



/*
using UnityEngine;

public class Temple : MonoBehaviour, IKillable
{
    // -------- Stats (same pattern as enemies) --------
    public float health = 20f;
    public float max_health = 20f;

    // -------- Visuals --------
    public Sprite intactSprite;
    public Sprite destroyedSprite;

    private SpriteRenderer spriteRenderer;
    private Health hp;
    private bool destroyed = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hp = GetComponentInChildren<Health>();

        // SAME initialization pattern as Water_Slime
        hp.set_max_hp(max_health);
        hp.set_hp(health);

        if (spriteRenderer != null && intactSprite != null)
            spriteRenderer.sprite = intactSprite;
    }

    // -------- IKillable (EXACT Water_Slime pattern) --------

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

        // Optional: stop further interactions
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Debug.Log("Temple destroyed!");
    }
}*/


