using UnityEngine;

public class TempleDrawnEnemy : MonoBehaviour, IKillable
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public LayerMask waterLayer;
    public float skinWidth = 0.05f;

    [Header("Target")]
    public Transform templePosition;

    [Header("Attack")]
    public float attackInterval = 1.0f;
    public float attackRadius = 1.0f;
    public float damage = 1.0f;
    public LayerMask enemyLayer;
    public Transform attackPoint;

    [Header("Health")]
    public float maxHealth = 3f;
    private float health;

    private Animator animator;
    private CapsuleCollider2D capsule;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveDirection;
    private float attackTimer;
    private bool isAttacking;

    void Start()
    {
        capsule = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        health = maxHealth;
        attackTimer = attackInterval;
    }

    void Update()
    {
        if (health <= 0) return;

        if (isAttacking)
        {
            HandleAttack();
            UpdateSpriteFacing();
            return;
        }

        moveDirection =
            ((Vector2)templePosition.position - (Vector2)transform.position).normalized;

        float moveDist = moveSpeed * Time.deltaTime;

        if (CanMove(moveDirection, moveDist))
        {
            transform.position += (Vector3)(moveDirection * moveDist);
        }
        else
        {
            StartAttacking();
        }

        UpdateSpriteFacing();
    }

    void StartAttacking()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);
        attackTimer = attackInterval;
    }

    bool CanMove(Vector2 dir, float dist)
    {
        Bounds b = capsule.bounds;

        RaycastHit2D hit = Physics2D.BoxCast(
            b.center,
            b.size,
            0f,
            dir,
            dist + skinWidth,
            waterLayer
        );

        return hit.collider == null;
    }

    void HandleAttack()
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D c in hits)
        {
            IKillable killable = c.GetComponentInParent<IKillable>();

            if (killable != null)
            {
                killable.hit(damage);
            }
        }
    }

    void UpdateSpriteFacing()
    {
        if (moveDirection.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (moveDirection.x < -0.01f)
            spriteRenderer.flipX = true;
    }

    // -------- IKillable --------

    public void hit(float dmg)
    {
        health -= dmg;

        if (health <= 0)
            OnDeath();
    }

    public void OnDeath()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
