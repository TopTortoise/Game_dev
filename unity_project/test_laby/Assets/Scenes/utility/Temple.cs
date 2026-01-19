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
}



/*using UnityEngine;
using UnityEngine.UI;

public class Temple : MonoBehaviour, IKillable
{
    [Header("Health")]
    public float maxHealth = 20f;
    private float health;

    [Header("Visuals")]
    public Sprite intactSprite;
    public Sprite destroyedSprite;
    private SpriteRenderer spriteRenderer;

    [Header("UI")]
    public Image healthBarFill; // optional

    private bool destroyed = false;

    void Start()
    {
        health = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && intactSprite != null)
            spriteRenderer.sprite = intactSprite;

        UpdateHealthUI();
    }

    // -------- IKillable --------

    public void hit(float damage)
    {
        if (destroyed) return;

        health -= damage;
        health = Mathf.Max(health, 0);

        Debug.Log("Temple hit by Enemy" + health);
        UpdateHealthUI();

        if (health <= 0)
            OnDeath();
    }

    public void OnDeath()
    {
        destroyed = true;

        if (spriteRenderer != null && destroyedSprite != null)
            spriteRenderer.sprite = destroyedSprite;

        Debug.Log("Temple destroyed!");
    }

    // -------- UI --------

    void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = health / maxHealth;
        }
    }
}*/

