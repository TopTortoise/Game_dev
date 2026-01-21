using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class Blue_staff : IWeapon
{
    public float offset;
    public GameObject projectile;
    public Transform AttackPoint;
    public InputAction AimAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = new();
        effects.Add(new SlowEffect(0.1f,2f));
        AttackAction.Enable();
        stats.damage = 1;
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
        AimAction.Disable();
        AttackAction.Disable();
        is_equipped = false;
    }

    void Update()
    {
        if (is_equipped)
        {

            Vector2 aimInput = AimAction.ReadValue<Vector2>();
            Vector3 dir;

            if (aimInput.sqrMagnitude > 0.1f)
            {
                dir = new Vector3(aimInput.x, aimInput.y, 0).normalized;  // gamepad
            }
            else
            {
                Vector3 mouse = Mouse.current.position.ReadValue();
                mouse.z = Camera.main.WorldToScreenPoint(transform.position).z;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouse);
                dir = (worldPos - transform.position).normalized;         // mouse
            }

            float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotZ + offset);

        }
    }
//implemet on Hit_logic
    public override void Attack()
    {
        
        OnAttack();
        if (!isRotating)
        {
            Vector2 aimInput = AimAction.ReadValue<Vector2>();
            Vector3 direction;

            if (aimInput.sqrMagnitude > 0.1f)
            {
                direction = new Vector3(aimInput.x, aimInput.y, 0).normalized;  // gamepad
            }
            else
            {
                Vector3 mouse = Mouse.current.position.ReadValue();
                mouse.z = Camera.main.WorldToScreenPoint(AttackPoint.position).z;
                Vector3 target = Camera.main.ScreenToWorldPoint(mouse);
                direction = (target - AttackPoint.position).normalized;         // mouse
            }

            GameObject bullet = Instantiate(projectile, AttackPoint.position, Quaternion.identity);
            AudioManager.Instance.Play(AudioManager.SoundType.Shoot);
            bullet.GetComponent<projectile>().OnHit += HandleonHit;
            // Debug.Log( );
            bullet.GetComponent<projectile>().direction = direction;   // speed

            StartCoroutine(RotateCoroutine());
        }

    }

    void HandleonHit(HitContext con){
      foreach(IWeaponEffect effect in effects)
      {
        if(effect is IOnHitEffect onhit){
         onhit.Apply(con); 
        }
        else if( effect is StatusEffect stateffect ){
          IEnemy enemy = con.target.GetComponent<IEnemy>();
          if(enemy != null){
            Debug.Log("handlign effects "+ stateffect);
            enemy.ApplyEffect(stateffect.createEffect());
          }
        }
      }       

    }


    public float rotationAngle = 45f; // degrees

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

}
