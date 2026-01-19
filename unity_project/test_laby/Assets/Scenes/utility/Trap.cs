using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 1f;
    public float damageInterval = 1f;

    private float damageTimer = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        ghost player = other.GetComponentInParent<ghost>();
        if (player == null) return;

        damageTimer -= Time.deltaTime;
        if (damageTimer > 0f) return;

        IKillable killable = player.GetComponent<IKillable>();
        if (killable != null)
        {
            killable.hit(damage);
            damageTimer = damageInterval;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Reset timer so damage doesn't instantly apply on re-enter
        if (other.GetComponentInParent<ghost>() != null)
        {
            damageTimer = 0f;
        }
    }
}

