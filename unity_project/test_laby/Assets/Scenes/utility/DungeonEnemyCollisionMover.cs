using UnityEngine;
using System.Collections.Generic;

public class DungeonEnemyCapsuleMover : MonoBehaviour
{
    public float moveSpeed = 2f;
    public LayerMask waterLayer;
    public float skinWidth = 0.02f;

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
        moveDirection = directions[Random.Range(0, directions.Length)];
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
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
        UpdateSpriteFacing();

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

        if (validDirs.Count == 0)
        {
            moveDirection = -moveDirection;
        }
        else
        {
            moveDirection = validDirs[Random.Range(0, validDirs.Count)];
        }
    }

    void UpdateSpriteFacing()
    {
        if (moveDirection.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (moveDirection.x < -0.01f)
            spriteRenderer.flipX = true;
    }
}
