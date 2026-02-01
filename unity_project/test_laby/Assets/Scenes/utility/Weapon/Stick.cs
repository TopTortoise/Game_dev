using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Stick : IWeapon
{
    public float offset;
    public bool is_attacking = false;
    private bool canDealDamage = false;

    public Transform AttackPoint;
    public InputAction AimAction;

    public float swingAngle = 120f;   // total arc
    public float swingRadius = 0.5f;  // how far weapon moves while swinging

    private Vector2 aimInput;

    void Awake()
    {
        stats.damage = 1f;
        stats.attackspeed = 0.5f;
    }

    public override void onEquip()
    {
        AimAction.Enable();
        AttackAction.Enable();
        is_equipped = true;
    }

    public override void onUnequip()
    {
        AttackAction.Disable();
        AimAction.Disable();
        is_equipped = false;
    }

    public override void Attack()
    {
        if (!is_attacking)
        {
            AudioManager.Instance.Play(AudioManager.SoundType.Attack);
            OnAttack();
            StartCoroutine(stick_swing());
        }
    }

    IEnumerator stick_swing()
    {
        is_attacking = true;
        canDealDamage = true;

        // --- Determine aim direction (controller OR mouse) ---
        Vector3 dir;
        Vector2 aim = AimAction.ReadValue<Vector2>();

        if (aim.sqrMagnitude > 0.1f)
        {
            dir = new Vector3(aim.x, aim.y, 0).normalized;
        }
        else
        {
            Vector3 mouse = Mouse.current.position.ReadValue();
            mouse.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 world = Camera.main.ScreenToWorldPoint(mouse);
            dir = (world - transform.position).normalized;
        }

        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + offset;

        float halfSwing = swingAngle * 0.5f;
        float startAngle = baseAngle - halfSwing;
        float endAngle   = baseAngle + halfSwing;

        Vector3 startPos = transform.localPosition;

        float duration = stats.attackspeed;
        float t = 0f;

        // --- Swing arc ---
        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float angle = Mathf.Lerp(startAngle, endAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // small circular motion to feel physical
            Vector3 swingOffset =
                new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad),
                            Mathf.Sin(angle * Mathf.Deg2Rad),
                            0f) * swingRadius;

            transform.localPosition = startPos + swingOffset;

            yield return null;
        }

        canDealDamage = false;

        // --- Return weapon to hand ---
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / (duration * 0.4f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, t);
            yield return null;
        }

        transform.localPosition = startPos;
        is_attacking = false;
    }

    void Update()
    {
        // Same idle aiming logic as Spear
        if (!is_attacking && is_equipped)
        {
            aimInput = AimAction.ReadValue<Vector2>();
            Vector3 dir;

            if (aimInput.sqrMagnitude > 0.1f)
            {
                dir = new Vector3(aimInput.x, aimInput.y, 0).normalized;
            }
            else
            {
                Vector3 mouse = Mouse.current.position.ReadValue();
                mouse.z = Camera.main.WorldToScreenPoint(transform.position).z;
                Vector3 world = Camera.main.ScreenToWorldPoint(mouse);
                dir = (world - transform.position).normalized;
            }

            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotZ + offset);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!canDealDamage) return;

        if (collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            return;

        IKillable obj = collider.GetComponent<IKillable>();
        if (obj != null && collider.gameObject.tag != "torch")
        {
            obj.hit(stats.damage);

            IEnemy enemy = collider.GetComponent<IEnemy>();
            if (enemy != null)
            {
                OnHit(enemy);
            }
        }
    }
}



/*using UnityEngine;
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
    Debug.Log("stick attack");
    if (!isRotating)
    {
      Debug.Log("stick attack");
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
}*/
