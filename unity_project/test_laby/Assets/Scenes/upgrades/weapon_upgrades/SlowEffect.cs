public class SlowEffect : StatusEffect
{
  private float slowMultiplier;
 

  public override StatusEffect createEffect(){
    return new SlowEffect(slowMultiplier,duration);
  }

  public SlowEffect(float multiplier, float duration)
  {
    slowMultiplier = multiplier;
    this.duration = duration;
    unique = true;
  }

  public override void OnApply(IEnemy target)
  {
    target.speed *= slowMultiplier;
  }

  public override void OnExpire(IEnemy target)
  {
    target.speed /= slowMultiplier;
  }
}
