using UnityEngine;
using System.Collections;

public class TorchTurret : MonoBehaviour, IKillable
{
    [Header("Attack")]
    public float damage = 1f;
    public float attackInterval = 1.0f;
    public LayerMask enemyLayer;

    [Header("Range")]
    public Collider2D torchCollision; // trigger

    [Header("Ring Visual")]
    public LineRenderer ringRenderer;
    public float ringDuration = 0.4f;
    public int ringSegments = 64;

    [Header("Health (Health component driven)")]
    public float health = 5f;
    public float max_health = 5f;

    private float attackTimer;
    private Collider2D[] hitBuffer = new Collider2D[16];

    // -------- Components --------
    public Health hp;
    private bool destroyed = false;

    void Awake()
    {
        hp = GetComponentInChildren<Health>();

        
        if (hp != null)
        {
            hp.set_max_hp(max_health);
            hp.set_hp(health);
        }
    }

    void Start()
    {
        attackTimer = attackInterval;

        if (ringRenderer != null)
        {
            ringRenderer.positionCount = ringSegments + 1;
            ringRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (destroyed) return;

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackInterval;
        }
    }

    // ---------------- Attack ----------------

    void Attack()
    {
        ContactFilter2D filter = new ContactFilter2D
        {
            layerMask = enemyLayer,
            useLayerMask = true,
            useTriggers = true
        };

        int hitCount = Physics2D.OverlapCollider(
            torchCollision,
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

    // ---------------- Ring Visual ----------------

    IEnumerator ExpandRing()
    {
        ringRenderer.enabled = true;

        float time = 0f;
        float maxRadius = torchCollision.bounds.extents.x;

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

    

    public void hit(float damage)
    {
        if (destroyed) return;

        if (hp != null)
        {
            hp.change_health(damage);
            // GameManager.Instance.Torchpoint.Remove(gameObject.GetEntityId());

            // GameManager.Instance.Torchpoint.Add(gameObject.GetEntityId(),(gameObject.transform.position,hp.health));
        }
        
    }

    public void OnDeath()
    {
        if (destroyed) return;
        destroyed = true;

        Debug.Log("Torch destroyed");

        if (torchCollision) torchCollision.enabled = false;
        this.enabled = false;

        GameManager.Instance.Torchpoint.Remove(gameObject.GetEntityId());
        Destroy(gameObject);
    }
}




/*using UnityEngine;
using System.Collections;

public class TorchTurret : MonoBehaviour
{
    [Header("Attack")]
    public float damage = 1f;
    public float attackInterval = 1.0f;
    public LayerMask enemyLayer;

    [Header("Range")]
    public Collider2D torchCollision; // MUST be a trigger

    [Header("Ring Visual")]
    public LineRenderer ringRenderer;
    public float ringDuration = 0.4f;
    public int ringSegments = 64;

    private float attackTimer;
    private Collider2D[] hitBuffer = new Collider2D[16];

    void Start()
    {
        attackTimer = attackInterval;

        if (ringRenderer != null)
        {
            ringRenderer.positionCount = ringSegments + 1;
            ringRenderer.enabled = false;
        }
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackInterval;
        }
    }

    void Attack()
    {
        ContactFilter2D filter = new ContactFilter2D
        {
            layerMask = enemyLayer,
            useLayerMask = true,
            useTriggers = true
        };

        int hitCount = Physics2D.OverlapCollider(
            torchCollision,
            filter,
            hitBuffer
        );

        for (int i = 0; i < hitCount; i++)
        {
            IKillable killable = hitBuffer[i].GetComponentInParent<IKillable>();
            if (killable != null)
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
        float maxRadius = torchCollision.bounds.extents.x;

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
}*/

