using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class Blue_staff : IWeapon
{
    public float offset;
    public GameObject projectile;
    public Transform AttackPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AttackAction.Enable();
        attackspeed = 0.5f;
        sign = -1;
    }

    void Update()
    {
        Vector3 mouse = Mouse.current.position.ReadValue();
        mouse.z = Camera.main.WorldToScreenPoint(transform.position).z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouse);
        Vector3 diff = worldPos - transform.position;

        float rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ + offset);
    }

    public override void Attack()
    {


        if (!isRotating)
        {
            Vector3 mouse = Mouse.current.position.ReadValue();
            mouse.z = Camera.main.WorldToScreenPoint(AttackPoint.position).z;

            Vector3 target = Camera.main.ScreenToWorldPoint(mouse);
            Vector3 direction = (target - AttackPoint.position).normalized;

            GameObject bullet = Instantiate(projectile, AttackPoint.position, Quaternion.identity);

            bullet.GetComponent<projectile>().direction = direction;   // speed

            StartCoroutine(RotateCoroutine());
        }

    }

    public float rotationAngle = 45f; // degrees

    private bool isRotating = false;

    private IEnumerator RotateCoroutine()
    {
        isRotating = true;

        float halfDuration = attackspeed / 2f;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 0, sign * rotationAngle);

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
