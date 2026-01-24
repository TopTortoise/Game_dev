using UnityEngine;
[CreateAssetMenu(menuName = "Weapon Effects/SlowEffect")]
public class SlowEffect : StatusEffect
{
  public float slowMultiplier = 0.5f;

  public override void OnApply(IEnemy target)
  {
    target.speed *= slowMultiplier;
  }

  public override void OnExpire(IEnemy target)
  {
    target.speed /= slowMultiplier;
  }
  public override string GetDescription()
    {
        
        float percent = (1f - slowMultiplier) * 100f;
        return $"Slow ({percent}%)";
    }
}
