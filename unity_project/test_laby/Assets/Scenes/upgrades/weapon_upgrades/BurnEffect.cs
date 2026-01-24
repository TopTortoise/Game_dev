using UnityEngine;
[CreateAssetMenu(menuName = "Weapon Effects/BurnEffect")]
public class BurnEffect : StatusEffect
{
  public float damagePerSecond = 0.5f;
 

  int counter = 0;
  public override void OnApply(IEnemy target){
    elapsed = 0;
  }
  public override void OnTick(IEnemy target, float deltaTime)
  {
    Debug.Log("im burning "+counter+" dur is "+duration+" elapsed is"+elapsed);
    counter++;
    target.hit(damagePerSecond * deltaTime);
  }
  public override string GetDescription()
    {
        return $"Burn ({damagePerSecond} DPS)";
    }
}
  