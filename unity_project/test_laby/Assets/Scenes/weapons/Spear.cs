using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class Spear : IWeapon

{
  public bool equipped = true;
  public float offset;
  public bool is_attacking = false;
  private bool canDealDamage = false;
  public Transform AttackPoint;
  public InputAction AimAction;
  private Vector2 aimInput;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Awake()
  {
    stats.damage = 1f;
    stats.attackspeed = 1f;

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
      Debug.Log("attack");
      OnAttack();
      StartCoroutine(spear_poke());
    }
  }


  IEnumerator spear_poke()
  {
    is_attacking = true;
    canDealDamage = true;
    float pokeDistance = 1f;     // how far the spear moves forward
                                 // --- Calculate direction toward mouse in world space ---
    Vector3 dir;

    Vector2 aimInput = AimAction.ReadValue<Vector2>();
    if (aimInput.sqrMagnitude > 0.1f)
    {
      // Gamepad attack direction
      dir = new Vector3(aimInput.x, aimInput.y, 0).normalized;
    }
    else
    {
      // Mouse attack direction
      Vector3 mouseScreen = Mouse.current.position.ReadValue();
      mouseScreen.z = Camera.main.WorldToScreenPoint(transform.position).z;
      Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
      dir = (mouseWorld - transform.position).normalized;
    }

    // Rotate transform to face mouse
    // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    // transform.rotation = Quaternion.Euler(0f, 0f, angle);

    Vector3 startPos = transform.localPosition;                 // relative to player
    Vector3 forwardPos = startPos + dir * pokeDistance;     // poke forward
    Vector3 retractPos = startPos - dir * (pokeDistance * 0.2f); // optional recoil

    float phaseDuration = stats.attackspeed / 3f;
    float t = 0f;

    // --- Phase 1: poke forward ---
    t = 0f;
    while (t < 1f)
    {
      t += Time.deltaTime / phaseDuration;
      transform.localPosition = Vector3.Lerp(startPos, forwardPos, t);
      yield return null;
    }
    canDealDamage = false;

    // --- Phase 2: retract ---
    t = 0f;
    while (t < 1f)
    {
      t += Time.deltaTime / phaseDuration;
      transform.localPosition = Vector3.Lerp(forwardPos, retractPos, t);
      yield return null;
    }

    // --- Phase 3: return ---
    t = 0f;
    while (t < 1f)
    {
      t += Time.deltaTime / phaseDuration;
      transform.localPosition = Vector3.Lerp(retractPos, startPos, t);
      yield return null;
    }

    is_attacking = false;
  }

  // Update is called once per frame
  void Update()
  {
    if (!is_attacking && is_equipped)
    {
      aimInput = AimAction.ReadValue<Vector2>();
      Vector3 dir;
      if (aimInput.sqrMagnitude > 0.1f)
      {
        // --- Gamepad aiming ---
        dir = new Vector3(aimInput.x, aimInput.y, 0).normalized;
      }
      else
      {
        // --- Mouse aiming ---
        Vector3 mouse = Mouse.current.position.ReadValue();
        mouse.z = Camera.main.WorldToScreenPoint(transform.position).z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouse);
        dir = (worldPos - transform.position).normalized;
      }
      float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, rotZ + offset);
    }
  }

  void OnTriggerEnter2D(Collider2D collider)
  {
    if (canDealDamage)
    {

      Debug.Log("hit something " + collider);


      if (collider.gameObject.layer == LayerMask.NameToLayer("Water"))
      {
        return;
      }
      IKillable obj = collider.gameObject.GetComponent<IKillable>();
      if (obj != null)
      {

        obj.hit(stats.damage);
        if (collider.gameObject.GetComponent<IEnemy>() != null)
        {
          OnHit(collider.gameObject.GetComponent<IEnemy>());

        }
      }
    }
  }
}
