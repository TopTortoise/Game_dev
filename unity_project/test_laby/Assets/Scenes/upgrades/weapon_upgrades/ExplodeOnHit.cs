using UnityEngine;
[CreateAssetMenu(menuName = "Weapon Effects/ExplodeEffect")]
public class ExplodeOnHit : IOnHitEffect
{

  public float radius;
  public float damage;
  public LayerMask hit_layer;

  public IOnHitEffect createEffect()
  {
    return new ExplodeOnHit(radius, damage);
  }



  public ExplodeOnHit(float radius, float damage)
  {
    this.radius = radius;
    this.damage = damage;
  }

  public override void Apply(HitContext context)
  {
    var hits = Physics2D.OverlapCircleAll(
          context.hitPoint, radius, hit_layer);

    foreach (var hit in hits)
    {
      var target = hit.GetComponent<IEnemy>();
      if (target != null)
      {
        target.hit(damage);
        Debug.Log("exhit " + hit + " at " + hit.transform.position + " with " + damage + "damage");

      }
    }
    Debug.Log("explosions!!!!!!!! at " + context.hitPoint);
    ExplosionVisualizer.Instance.ShowCircle(context.hitPoint, radius, 1f);
  }

  public override string GetDescription()
  {
    return $"Explodes ({damage} Dmg, {radius}m)";
  }

}
