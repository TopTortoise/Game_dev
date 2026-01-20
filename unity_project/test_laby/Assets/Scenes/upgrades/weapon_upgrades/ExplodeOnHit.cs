using UnityEngine;
public class ExplodeOnHit : IOnHitEffect
{

  private float radius;
  private float damage;

  public ExplodeOnHit(float radius, float damage)
  {
    this.radius = radius;
    this.damage = damage;
  }

  public void Apply(HitContext context)
  {
    Debug.Log("explosions!!!!!!!!");
    var hits = Physics2D.OverlapCircleAll(
        context.hitPoint, radius);

    foreach (var hit in hits)
    {
      var target = hit.GetComponent<IEnemy>();
      if (target != null)
        target.hit(damage);
    }
    ExplosionVisualizer.Instance.ShowCircle(context.hitPoint, radius, 1f);
  }


}
