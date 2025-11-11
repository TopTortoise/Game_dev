using UnityEngine;

public class detectionarea : MonoBehaviour
{

    public EnemyController parent;
    public float radius = 2.0f;
    public Color color = new Color(255, 0, 0, 128);

    private void Awake()
    {
        parent = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            parent.SetTarget(other.transform);
        }
        // You can call back to parent logic here:
        // GetComponentInParent<Enemy>().OnTargetEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parent.ResetTarget();
        }
        // GetComponentInParent<Enemy>().OnTargetExit(other);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

