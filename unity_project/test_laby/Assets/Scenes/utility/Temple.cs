
using UnityEngine;
using System.Collections;
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


    [Header("Ultimate Attack")]
    public float damage = 1f;
    public float attackInterval = 10f;
    public LayerMask enemyLayer;
    public Collider2D attackCollider; // trigger

    [Header("Ring Visual")]
    public LineRenderer ringRenderer;
    public float ringDuration = 0.6f;
    public int ringSegments = 64;

    private float attackTimer;
    private Collider2D[] hitBuffer = new Collider2D[32];

    [Header("Upgrade Balancing")]
    private int baseUpgradeCost = 100;
    private float healthPerLevel = 25f;
    private float damagePerLevel = 0.25f;
    private float cooldownReductionPerLevel = 2f;
    public float minCooldown = 1f;

    private float baseUltDamage;
    private float baseUltInterval;

    void Awake()
    {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        hp = GetComponentInChildren<Health>();

        health = GameState.Instance.templeHealth;
        max_health = GameState.Instance.templeHealth;
        Debug.Log("Temple Awake - GameState Health: " + health);
        hp.set_max_hp(max_health);
        hp.set_hp(health);

        if (spriteRenderer != null && intactSprite != null)
            spriteRenderer.sprite = intactSprite;

        regenTimer = regenInterval;
        attackTimer = attackInterval;

        if (ringRenderer != null)
        {
            ringRenderer.positionCount = ringSegments + 1;
            ringRenderer.enabled = false;
        }
        destroyed = false;

        baseUltDamage = 1;
        baseUltInterval = 10f;

        RecalculateStats();

    }

    void Update()
    {
        if (destroyed) return;


        regenTimer -= Time.deltaTime;
        if (regenTimer <= 0f)
        {
             Debug.Log($"[Temple Regen] +{regenAmount} HP at time {Time.time:F1}");

            // Negative damage = healing
            hp.change_health(-regenAmount);

            regenTimer = regenInterval;
        }
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f && GameState.Instance.enemyWaveActive)
        {
            UltimateAttack();
            attackTimer = attackInterval;
        }

    }

    public void ResetTemple()
    {
        destroyed = false;
        Debug.Log("Reset Temple called");
        if (spriteRenderer != null && destroyedSprite != null)
            spriteRenderer.sprite = intactSprite;
        health = GameState.Instance.templeHealth;
        max_health = GameState.Instance.templeHealth;
        hp.set_max_hp(max_health);
        hp.set_hp(health);
    }

    void Start()
    {
        if (spriteRenderer != null && destroyedSprite != null)
            spriteRenderer.sprite = intactSprite;
        health = GameState.Instance.templeHealth;
        max_health = GameState.Instance.templeHealth;
        destroyed = false;
        hp.set_max_hp(max_health);
        hp.set_hp(health);
    }

    public void UpgradeMaxHealth(float amount)
    {
        max_health += amount;
        health += amount;
        hp.set_max_hp(max_health);

        hp.set_hp(health);

        GameState.Instance.SetTempleHealth(max_health);
        GameState.Instance.SetCurrentTempleHealth(health);

        Debug.Log($"Temple upgraded! New max HP: {max_health}");
    }

    void UltimateAttack()
    {
        ContactFilter2D filter = new ContactFilter2D
        {
            layerMask = enemyLayer,
            useLayerMask = true,
            useTriggers = true
        };

        int hitCount = Physics2D.OverlapCollider(
            attackCollider,
            filter,
            hitBuffer
        );

        for (int i = 0; i < hitCount; i++)
        {
            IKillable killable = hitBuffer[i].GetComponentInParent<IKillable>();
            if (killable != null && killable != this)
            {
                killable.hit(damage);
            }
        }

        if (ringRenderer != null)
            StartCoroutine(ExpandRing());
    }
    IEnumerator ExpandRing()
    {

        ringRenderer.enabled = true;

        float time = 0f;
        float maxRadius = attackCollider.bounds.extents.x;

        while (time < ringDuration)
        {
            float radius = Mathf.Lerp(0f, maxRadius, time / ringDuration);
            DrawRing(radius);
            time += Time.deltaTime;
            yield return null;
        }

        ringRenderer.enabled = false;
    }

    void DrawRing(float radius)
    {
        if (CameraShake.Instance != null) CameraShake.Instance.Shake(1.2f, 0.04f);
        AudioManager.Instance.Play(AudioManager.SoundType.Temple_Attack);
        for (int i = 0; i <= ringSegments; i++)
        {
            float angle = i * Mathf.PI * 2f / ringSegments;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );

            ringRenderer.SetPosition(i, transform.position + offset);
        }
    }

    public void RecalculateStats()
    {
        float baseHealth = GameState.Instance.templeHealth; 
        float calculateMaxHealth = baseHealth + ((GameState.Instance.levelHealth - 1) * healthPerLevel);

        if (max_health != calculateMaxHealth)
        {
            float diff = calculateMaxHealth - max_health;
            UpgradeMaxHealth(diff);
        }

        damage = baseUltDamage + ((GameState.Instance.levelUltDamage - 1) * damagePerLevel);

        float baseUltInterval = 30f;
        float newInterval = baseUltInterval - ((GameState.Instance.levelUltCooldown - 1) * 2f);
        attackInterval = Mathf.Max(newInterval, 18f);


    }

    public int GetUpgradeCost(int currentLevel)
    {
        return Mathf.RoundToInt(baseUpgradeCost + (currentLevel - 1) * 50);
    }


    public void hit(float damage)
    {
        Debug.Log("temple got hit " + damage  ); 
        if (destroyed) return;

        hp.change_health(damage);
    }

    public void OnDeath()
    {
        if (destroyed) return;
        destroyed = true;

        if (spriteRenderer != null && destroyedSprite != null)
            spriteRenderer.sprite = destroyedSprite;

        GameOverManager goManager = FindFirstObjectByType<GameOverManager>();
        if (goManager != null)
        {
            goManager.StartGameOver();
        }
        else
        {
            Debug.LogWarning("GameOverManager not Found!");
        }

        Debug.Log("Temple destroyed!");

    }
}

