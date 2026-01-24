using UnityEngine;
[CreateAssetMenu(menuName = "Weapon Effects/ExplodeEffect")]
public class Lightning : IOnHitEffect
{

  public float radius;
  public float damage;

  public override void Apply(HitContext context)
  {
    var hits = Physics2D.OverlapCircleAll(
        context.hitPoint, radius, LayerMask.NameToLayer("Enemies"));
    int count = 0;
    foreach (var hit in hits)
    {
      var target = hit.GetComponent<IEnemy>();
      if (target != null){
        target.hit(damage);
        count++;

      }
      if(count == 2){
        break;
      }
      target.GetEntityId();
    }
    ExplosionVisualizer.Instance.ShowCircle(context.hitPoint, radius, 1f);
  }

  public override string GetDescription() 
    {
        return $"Lightning ({damage} Dmg)";
    }


}
