using UnityEngine;
using System.Collections;

public class TempleDrawnEnemy : IEnemy
{

  // -------- Movement --------
  public LayerMask waterLayer;
  public float skinWidth = 0.05f;
  public Transform templePosition;

  // -------- Attack --------
  public float attackInterval = 1.0f;
  public float attackRadius = 1.0f;
  public Transform Attackpoint;
  public LayerMask enemy_layer;

  // -------- Components --------
  private CapsuleCollider2D capsule;
  private Animator anim;
  private SpriteRenderer rend;
  private Health hp;

  // -------- State --------
  private Vector2 moveDirection;
  private float attackTimer;
  private bool isAttacking;

  void Awake()
  {
    // -------- Stats (same pattern as Water_Slime) --------
    speed = 2f;
    damage = 1f;
    health = 3f;
    max_health = 3f;
    //------------------------------------------------------
    capsule = GetComponent<CapsuleCollider2D>();
    anim = GetComponentInChildren<Animator>();
    rend = GetComponentInChildren<SpriteRenderer>();
    hp = GetComponentInChildren<Health>();

    // SAME initialization style as Water_Slime
    hp.set_max_hp(max_health);
    hp.set_hp(health);

    attackTimer = attackInterval;
  }

  void Update()
  {
    handleEffects();
    if (health <= 0) return;

    if (isAttacking)
    {
      HandleAttack();
      return;
    }

    moveDirection =
        ((Vector2)templePosition.position - (Vector2)transform.position).normalized;

    float moveDist = speed * Time.deltaTime;

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

  // -------- Movement Blocking --------

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

  // -------- Attacking --------

  void StartAttacking()
  {
    isAttacking = true;
    attackTimer = attackInterval;

    anim.SetBool("isAttacking", true);
  }

  void HandleAttack()
  {
    attackTimer -= Time.deltaTime;
    if (attackTimer > 0f) return;

    Attack();
    attackTimer = attackInterval;
  }

  public void Attack()
  {
    Collider2D[] colliders =
        Physics2D.OverlapCircleAll(Attackpoint.position, attackRadius, enemy_layer);

    foreach (Collider2D target in colliders)
    {
      IKillable killable = target.GetComponentInParent<IKillable>();
      if (killable != null && killable.Equals(this))
      {
        killable.hit(damage);
      }
    }
  }

  // -------- Visuals --------

  void UpdateSpriteFacing()
  {
    if (!rend) return;

    if (moveDirection.x > 0.01f)
      rend.flipX = false;
    else if (moveDirection.x < -0.01f)
      rend.flipX = true;
  }

  // -------- IKillable (EXACT Water_Slime style) --------

  public override void hit(float damage)
  {
    // identical to Water_Slime
    hp.change_health(damage);
    if (hp.health <= 0) OnDeath();
  }

  public override void OnDeath()
  {
    anim.SetBool("isDead", true);
    Debug.Log("TempleDrawnEnemy Died");
    StartCoroutine(DeathRoutine(1.5f));
    // Optional: loot, effects, animation

  }

  IEnumerator DeathRoutine(float duration)
  {
    Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
    yield return new WaitForSeconds(duration);
    Debug.Log($"Ended at {Time.time}");
    Destroy(gameObject);
  }

  // -------- Debug --------

  void OnDrawGizmos()
  {
    if (Attackpoint)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(Attackpoint.position, attackRadius);
    }
  }
}

/*
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
            IKillable killable = c.GetComponent<IKillable>();

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
}*/
