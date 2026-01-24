using UnityEngine;
[CreateAssetMenu(menuName = "Weapon Effects/ExplodeEffect")]
public class ExplodeOnHit : IOnHitEffect
{

  public float radius;
  public float damage;

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
    Debug.Log("explosions!!!!!!!!");
    var hits = Physics2D.OverlapCircleAll(
          context.hitPoint, radius, LayerMask.NameToLayer("Enemies"));

    foreach (var hit in hits)
    {
      var target = hit.GetComponent<IEnemy>();
      if (target != null)
        target.hit(damage);
    }
    ExplosionVisualizer.Instance.ShowCircle(context.hitPoint, radius, 1f);
  }

  public override string GetDescription()
    {
        return $"Explodes ({damage} Dmg, {radius}m)";
    }

}
