using UnityEngine;
public abstract class IEnemy : MonoBehaviour
{
    public float speed;
    public float damage;
    public float health;
    public float max_health;
    public Transform target;


    public void ResetTarget()
    {
        target = null;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public Transform getTarget(){
      return target;
    }
}
