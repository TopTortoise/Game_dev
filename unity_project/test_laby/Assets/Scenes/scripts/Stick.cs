using UnityEngine;
using UnityEngine.InputSystem;

using System.Collections;
public class Stick : MonoBehaviour
{

    public InputAction AttackAction;
    public Transform Attackpoint;
    public float radius = 5f;
    public LayerMask enemy_layer;
    public int sign = -1;
    CapsuleCollider2D hb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hb = GetComponent<CapsuleCollider2D>();
        AttackAction.Enable();

    }

    public float rotationAngle = 90f; // degrees
    public float duration = 0.5f;       // time for one back-and-forth

    private bool isRotating = false;

    void Update()
    {
        if (AttackAction.IsPressed() && !isRotating)
        {
            StartCoroutine(RotateCoroutine());
            Attack();
        }
    }

    private IEnumerator RotateCoroutine()
    {
        isRotating = true;

        float halfDuration = duration / 2f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 0, sign*rotationAngle);

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

    void Attack()
    {
      // Debug.Log("Stick Attack");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(Attackpoint.position, radius, enemy_layer);
        
        foreach (Collider2D enemy in enemies)
        {
            Debug.Log("Stick hit"+enemy.name);
            enemy.GetComponent<IKillable>().hit(1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Attackpoint.position, radius);
    }
    // Update is called once per frame
}
