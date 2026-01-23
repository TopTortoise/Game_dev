using UnityEngine;
[CreateAssetMenu(menuName = "Weapon Effects/BurnEffect")]
public class BurnEffect : StatusEffect
{
  public float damagePerSecond = 0.5f;
  

  public override void OnTick(IEnemy target, float deltaTime)
  {
    target.hit(damagePerSecond * deltaTime);
  }
}
