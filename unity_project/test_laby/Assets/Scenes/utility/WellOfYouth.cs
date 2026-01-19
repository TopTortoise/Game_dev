using UnityEngine;

public class WellOfYouth : MonoBehaviour
{
    [Header("Healing")]
    public float healAmount = 1f;
    public float healInterval = 0.5f;

    private float healTimer = 0f;

    void OnTriggerStay2D(Collider2D other)
    {
        healTimer -= Time.deltaTime;
        if (healTimer > 0f) return;

        IKillable killable = other.GetComponentInParent<IKillable>();
        if (killable != null)
        {
            Debug.Log("Player Healed + 1");
            // Negative damage = healing
            killable.hit(-healAmount);
            healTimer = healInterval;
        }
    }
}

