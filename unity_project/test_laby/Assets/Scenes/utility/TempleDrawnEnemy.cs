using UnityEngine;

public class TempleDrawnEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public LayerMask waterLayer;
    public float skinWidth = 0.05f;

    [Header("Target")]
    public Transform templePosition;

    [Header("Attack")]
    public float attackInterval = 1.0f;

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

        attackTimer = attackInterval;
    }

    void Update()
    {
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
            isAttacking = true;
            animator.SetBool("isAttacking", true);
        }

        UpdateSpriteFacing();
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
        Debug.Log($"{name} attacks the temple!");
    }

    void UpdateSpriteFacing()
    {
        if (moveDirection.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (moveDirection.x < -0.01f)
            spriteRenderer.flipX = true;
    }
}
