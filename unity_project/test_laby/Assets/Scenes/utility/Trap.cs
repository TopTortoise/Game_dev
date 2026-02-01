using UnityEngine;

public class Trap : MonoBehaviour
{
  [Header("Damage")]
  public float damage = 1f;
  public float damageInterval = 1f;

  private float damageTimer = 0f;

  private void OnTriggerStay2D(Collider2D other)
  {
    bool is_weapon = other.GetComponent<IWeapon>() != null || other.GetComponentInParent<IWeapon>() != null;
    Debug.Log("is weapon" + other.GetComponent<IWeapon>() + " bool is " + is_weapon);
    if (is_weapon)
    {
      Debug.Log("wepaon returning");
      return;
    }
    ghost player = other.GetComponentInParent<ghost>();
    if (player == null) return;

    damageTimer -= Time.deltaTime;
    if (damageTimer > 0f) return;

    IKillable killable = player.GetComponent<IKillable>();
    if (killable != null && !is_weapon)
    {
      Debug.Log("is weapon" + other.GetComponent<IWeapon>() + " bool is " + is_weapon);
      if (!player.isDashing) killable.hit(damage);
      damageTimer = damageInterval;
    }
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    // Reset timer 
    if (other.GetComponentInParent<ghost>() != null)
    {
      damageTimer = 0f;
    }
  }
}

