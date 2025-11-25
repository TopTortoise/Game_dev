using UnityEngine;
using System.Collections;
public class Blue_staff : IWeapon
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      AttackAction.Enable();
      attackspeed = 0.5f;
      sign = -1;
    }

    public override void Attack()
    {

        if (!isRotating){
          Debug.Log("stff");
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
