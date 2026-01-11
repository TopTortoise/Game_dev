using UnityEngine;
using System.Collections;
public class Stick : IWeapon
{
    // public float damage = 1f;
    // public float attackspeed = 0.5f;
    // public InputAction AttackAction;
    public Transform Attackpoint;
    public float radius = 5f;
    public LayerMask enemy_layer;
    CapsuleCollider2D hb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats.attackspeed = 0.5f;
        stats.damage = 1;
        hb = GetComponent<CapsuleCollider2D>();
        AttackAction.Enable();

    }


    public override void onEquip()
    {
        AttackAction.Enable();
        is_equipped = true;
    }

    public override void onUnequip()
    {
        AttackAction.Disable();
        is_equipped = false;
    }


    public float rotationAngle = -90f; // degrees

    private bool isRotating = false;

    private IEnumerator RotateCoroutine()
    {
        isRotating = true;

        float halfDuration = stats.attackspeed / 2f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 0, rotationAngle);

        // Rotate to +90 degrees
        float t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t / halfDuration);
            yield return null;
        }

        // Rotate back to original
        t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(endRotation, startRotation, t / halfDuration);
            yield return null;
        }

        transform.rotation = startRotation; // ensure exact original rotation
        isRotating = false;
    }

    public override void Attack()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateCoroutine());
            // Debug.Log("Stick Attack");
            Collider2D[] enemies = Physics2D.OverlapCircleAll(Attackpoint.position, radius, enemy_layer);

            StartCoroutine(Knockback(enemies));
            foreach (Collider2D enemy in enemies)
            {
                Debug.Log("Stick hit" + enemy.name);
                enemy.GetComponent<IKillable>().hit(stats.damage);
                // enemy.GetComponent<EnemyController>().speed *= -1;
            }

        }
    }
    private IEnumerator Knockback(Collider2D[] enemies)
    {
        foreach (Collider2D enemy in enemies)
        {
            enemy.GetComponent<IEnemy>().speed *= -4f;
        }
        yield return new WaitForSeconds(0.1f);
        foreach (Collider2D enemy in enemies)
        {
            enemy.GetComponent<IEnemy>().speed *= -0.25f;
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Attackpoint.position, radius);
    }
    // Update is called once per frame
}
