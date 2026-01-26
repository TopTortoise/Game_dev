using UnityEngine;

public class detectionarea : MonoBehaviour
{

    public IEnemy parent;
    public float radius = 2.0f;
    public Color color = new Color(255, 0, 0, 128);

    private void Awake()
    {
        parent = GetComponentInParent<IEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            parent.SetTarget(other.transform);
        }
      
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parent.ResetTarget();
        }
       
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

