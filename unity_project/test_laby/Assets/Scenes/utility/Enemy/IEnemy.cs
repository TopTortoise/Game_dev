using UnityEngine;
public abstract class IEnemy : MonoBehaviour, IKillable
{
    public float speed;
    public float damage;
    public float collision_damage = 1.0f;
    public float health;
    public float max_health;
    public Transform target;
    public void OnDeath(){}
    public void hit(float damage){}


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
