public class BurnEffect : StatusEffect
{
  private float damagePerSecond;
  public override StatusEffect createEffect()
  {
    return new SlowEffect(damagePerSecond, duration);
  }


  public BurnEffect(float dps, float duration)
  {
    this.damagePerSecond = dps;
    this.duration = duration;
    unique = true;
  }

  public override void OnTick(IEnemy target, float deltaTime)
  {
    target.hit(damagePerSecond * deltaTime);
  }
}
