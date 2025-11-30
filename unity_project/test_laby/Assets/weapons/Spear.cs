using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class Spear : IWeapon

{
    public bool equipped = true;
    public float offset;
    public bool is_attacking = false;
    public Transform AttackPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        damage = 1f;
        AttackAction.Enable();
        attackspeed = 0.5f;

    }



    public override void Attack()
    {
        if (!is_attacking)
        {
            StartCoroutine(spear_poke());
        }
    }


    IEnumerator spear_poke()
    {
        is_attacking = true;
        float pokeDistance = 1f;     // how far the spear moves forward
                                     // --- Calculate direction toward mouse in world space ---
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        mouseScreen.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Vector3 dir = (mouseWorld - transform.position).normalized; 

        // Rotate transform to face mouse
        // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Vector3 startPos = transform.localPosition;                 // relative to player
        Vector3 forwardPos = startPos + dir * pokeDistance;     // poke forward
        Vector3 retractPos = startPos - dir * (pokeDistance * 0.2f); // optional recoil

        float phaseDuration = attackspeed / 3f;
        float t = 0f;

        // --- Phase 1: poke forward ---
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / phaseDuration;
            transform.localPosition = Vector3.Lerp(startPos, forwardPos, t);
            yield return null;
        }

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
        if (!is_attacking)
        {

            Vector3 mouse = Mouse.current.position.ReadValue();
            mouse.z = Camera.main.WorldToScreenPoint(transform.position).z;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouse);
            Vector3 diff = worldPos - transform.position;

            float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotZ + offset);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (is_attacking)
        {

            Debug.Log("hit soemthing " + collider);
            IKillable obj = collider.gameObject.GetComponent<IKillable>();
            if (obj != null)
            {
                obj.hit(damage);
            }
        }
    }
}
