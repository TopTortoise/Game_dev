using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DungeonEnemyCapsuleMover : IEnemy
{
  // -------- Movement --------
  public LayerMask waterLayer;
  public float skinWidth = 0.02f;

  // -------- Contact Attack --------
  public float attackInterval = 0.5f;
  public float contactRadius = 0.5f;
  public LayerMask enemyLayer;

  // -------- Components --------
  private CapsuleCollider2D capsule;
  private SpriteRenderer spriteRenderer;
  private Health hp;

  // -------- State --------
  private Vector2 moveDirection;
  private float attackTimer;
  private Animator anim;

  private static readonly Vector2[] directions =
  {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        new Vector2(1, 1).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(-1, 1).normalized,
        new Vector2(-1, -1).normalized
    };

  void Awake()
  {
    // -------- Stats (Water_Slime style) --------
    speed = 2f;
    damage = 1.0f;
    health = 3f;
    max_health = 3f;

    capsule = GetComponent<CapsuleCollider2D>();
    spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    hp = GetComponentInChildren<Health>();
    anim = GetComponentInChildren<Animator>();

    // SAME initialization pattern as Water_Slime
    hp.set_max_hp(max_health);
    hp.set_hp(health);

    moveDirection = directions[Random.Range(0, directions.Length)];
    attackTimer = 0f;
  }

  void Update()
  {
    handleEffects();
    if (health <= 0) return;

    Move();
    HandleContactAttack();
    UpdateSpriteFacing();
  }

  // ---------------- Movement ----------------

  void Move()
  {
    float moveDist = speed * Time.deltaTime;

    if (CanMove(moveDirection, moveDist))
    {
      transform.Translate(moveDirection * moveDist);
    }
    else
    {
      ChooseNewDirection();
    }
  }

  bool CanMove(Vector2 dir, float dist)
  {
    RaycastHit2D hit = Physics2D.CapsuleCast(
        capsule.bounds.center,
        capsule.size,
        capsule.direction,
        0f,
        dir,
        dist + skinWidth,
        waterLayer
    );

    return hit.collider == null;
  }

  void ChooseNewDirection()
  {
    List<Vector2> validDirs = new List<Vector2>();

    foreach (Vector2 dir in directions)
    {
      if (Vector2.Dot(dir, moveDirection) < -0.8f)
        continue;

      if (CanMove(dir, speed * Time.deltaTime))
        validDirs.Add(dir);
    }

    moveDirection = validDirs.Count == 0
        ? -moveDirection
        : validDirs[Random.Range(0, validDirs.Count)];
  }

  // ---------------- Contact Attack ----------------

  void HandleContactAttack()
  {
    attackTimer -= Time.deltaTime;
    if (attackTimer > 0f) return;

    Collider2D[] hits = Physics2D.OverlapCircleAll(
        capsule.bounds.center,
        contactRadius,
        enemyLayer
    );

    foreach (Collider2D c in hits)
    {
      if (c.gameObject == gameObject) continue;

      IKillable killable = c.GetComponentInParent<IKillable>();
      if (killable != null && killable != this)
      {
        killable.hit(damage);
        attackTimer = attackInterval;
        break;
      }
    }
  }

  // ---------------- Visuals ----------------

  void UpdateSpriteFacing()
  {
    if (!spriteRenderer) return;

    if (moveDirection.x > 0.01f)
      spriteRenderer.flipX = false;
    else if (moveDirection.x < -0.01f)
      spriteRenderer.flipX = true;
  }

  // ---------------- IKillable (EXACT Water_Slime style) ----------------

  public void hit(float damage)
  {
    // identical to Water_Slime
    hp.change_health(damage);
  }

  public void OnDeath()
  {
    Debug.Log("DungeonEnemyCapsuleMover Died");
    anim.SetBool("isDead", true);
    StartCoroutine(DeathRoutine(1f));
  }

  IEnumerator DeathRoutine(float duration)
  {
    Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
    yield return new WaitForSeconds(duration);
    Debug.Log($"Ended at {Time.time}");
    Destroy(gameObject);
  }

  // ---------------- Debug ----------------

  void OnDrawGizmosSelected()
  {
    if (TryGetComponent(out CapsuleCollider2D c))
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(c.bounds.center, contactRadius);
    }
  }
}



/*using UnityEngine;
using System.Collections.Generic;

public class DungeonEnemyCapsuleMover : MonoBehaviour, IKillable
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public LayerMask waterLayer;
    public float skinWidth = 0.02f;

    [Header("Contact Attack")]
    public float damage = 1.0f;
    public float attackInterval = 0.5f;
    public float contactRadius = 0.5f;
    public LayerMask enemyLayer;

    [Header("Health")]
    public float maxHealth = 3f;

    private float health;
    private float attackTimer;

    private CapsuleCollider2D capsule;
    private Vector2 moveDirection;
    private SpriteRenderer spriteRenderer;

    private static readonly Vector2[] directions =
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right,
        new Vector2(1, 1).normalized,
        new Vector2(1, -1).normalized,
        new Vector2(-1, 1).normalized,
        new Vector2(-1, -1).normalized
    };

    void Start()
    {
        capsule = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        moveDirection = directions[Random.Range(0, directions.Length)];

        health = maxHealth;
        attackTimer = 0f;
    }

    void Update()
    {
        if (health <= 0) return;

        Move();
        HandleContactAttack();
        UpdateSpriteFacing();
    }

    // ---------------- Movement ----------------

    void Move()
    {
        float moveDist = moveSpeed * Time.deltaTime;

        if (CanMove(moveDirection, moveDist))
        {
            transform.Translate(moveDirection * moveDist);
        }
        else
        {
            ChooseNewDirection();
        }
    }

    bool CanMove(Vector2 dir, float dist)
    {
        RaycastHit2D hit = Physics2D.CapsuleCast(
            capsule.bounds.center,
            capsule.size,
            capsule.direction,
            0f,
            dir,
            dist + skinWidth,
            waterLayer
        );

        return hit.collider == null;
    }

    void ChooseNewDirection()
    {
        List<Vector2> validDirs = new List<Vector2>();

        foreach (Vector2 dir in directions)
        {
            if (Vector2.Dot(dir, moveDirection) < -0.8f)
                continue;

            if (CanMove(dir, moveSpeed * Time.deltaTime))
                validDirs.Add(dir);
        }

        moveDirection = validDirs.Count == 0
            ? -moveDirection
            : validDirs[Random.Range(0, validDirs.Count)];
    }

    // ---------------- Contact Attack ----------------

    void HandleContactAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            capsule.bounds.center,
            contactRadius,
            enemyLayer
        );

        foreach (Collider2D c in hits)
        {
            if (c.gameObject == gameObject) continue;

            IKillable killable = c.GetComponent<IKillable>();
            if (killable != null)
            {
                Debug.Log(c.gameObject);
                killable.hit(damage);
                attackTimer = attackInterval; // reset cooldown
                break; // only hit one per interval
            }
        }
    }


    // ---------------- Visuals ----------------

    void UpdateSpriteFacing()
    {
        if (!spriteRenderer) return;

        if (moveDirection.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (moveDirection.x < -0.01f)
            spriteRenderer.flipX = true;
    }

    // ---------------- IKillable ----------------

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

    // ---------------- Debug ----------------

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            GetComponent<CapsuleCollider2D>().bounds.center,
            contactRadius
        );
    }
}*/
