using UnityEngine;
using System.Collections;

public class WellOfYouth : MonoBehaviour
{
    [Header("Healing")]
    public float healAmount = 1f;
    public float healInterval = 0.5f;

    [Header("Range")]
    public Collider2D wellTrigger; // trigger

    [Header("Ring Visual")]
    public LineRenderer ringRenderer;
    public float ringDuration = 0.4f;
    public int ringSegments = 64;

    private float healTimer = 0f;

    void Start()
    {
        if (ringRenderer != null)
        {
            ringRenderer.positionCount = ringSegments + 1;
            ringRenderer.enabled = false;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Well healed"); 
        healTimer -= Time.deltaTime;
        if (healTimer > 0f) return;

        IKillable killable = other.GetComponentInParent<IKillable>();
        if (killable != null)
        {
            Debug.Log("Player Healed + " + healAmount);

            // Negative damage = healing
            killable.hit(-healAmount);
            healTimer = healInterval;

            if (ringRenderer != null)
                StartCoroutine(ExpandRing());
        }
    }

    // ---------------- Ring Visual ----------------

    IEnumerator ExpandRing()
    {
        ringRenderer.enabled = true;

        float time = 0f;
        float maxRadius = wellTrigger.bounds.extents.x;

        while (time < ringDuration)
        {
            float radius = Mathf.Lerp(0f, maxRadius, time / ringDuration);
            DrawRing(radius);
            time += Time.deltaTime;
            yield return null;
        }

        ringRenderer.enabled = false;
    }

    void DrawRing(float radius)
    {
        for (int i = 0; i <= ringSegments; i++)
        {
            float angle = i * Mathf.PI * 2f / ringSegments;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );

            ringRenderer.SetPosition(i, transform.position + offset);
        }
    }
}

