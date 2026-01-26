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
    
    speed = 2f;
    damage = 1.0f;
    health = 3f;
    max_health = 3f;

    capsule = GetComponent<CapsuleCollider2D>();
    spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    hp = GetComponentInChildren<Health>();
    anim = GetComponentInChildren<Animator>();

    
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
      if (killable != null && !killable.Equals(this))
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

  

  public override void hit(float damage)
  {
    
    hp.change_health(damage);
  }

  public override void OnDeath()
  {
    Debug.Log("DungeonEnemyCapsuleMover Died");
    anim.SetBool("isDead", true);
    StartCoroutine(DeathRoutine(1f));
    GameState.Instance.nrEnemiesDefeated++;
  }

  IEnumerator DeathRoutine(float duration)
  {
    Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
    yield return new WaitForSeconds(duration);
    Debug.Log($"Ended at {Time.time}");
    GetComponent<LootDropper>().DropLoot();
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


