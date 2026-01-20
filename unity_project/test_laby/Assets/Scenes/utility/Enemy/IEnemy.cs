using UnityEngine;
using System.Collections.Generic;
public abstract class IEnemy : MonoBehaviour, IKillable
{
  public float speed;
  public float damage;
  public float collision_damage = 1.0f;
  public float health;
  public float max_health;
  public Transform target;
  private List<StatusEffect> activeEffects = new();

  public void ApplyEffect(StatusEffect effect)
  {
    effect.OnApply(this);
    activeEffects.Add(effect);
  }

  public void handleEffects()
  {
    for (int i = activeEffects.Count - 1; i >= 0; i--)
    {
      if (activeEffects[i].Update(this, Time.deltaTime))
      {
        activeEffects[i].OnExpire(this);
        activeEffects.RemoveAt(i);
      }
    }
  }
  public virtual void OnDeath() { }
  public virtual void hit(float damage) { }


  public void ResetTarget()
  {
    target = null;
  }

  public void SetTarget(Transform t)
  {
    target = t;
  }

  public Transform getTarget()
  {
    return target;
  }
}
