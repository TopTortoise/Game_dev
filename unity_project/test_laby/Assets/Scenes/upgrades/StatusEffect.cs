public abstract class StatusEffect : IWeaponEffect
{
  public float duration;
  public bool unique;
  protected float elapsed;

  public virtual StatusEffect createEffect(){return null;}
  public virtual void OnApply(IEnemy target) { }
  public virtual void OnTick(IEnemy target, float deltaTime) { }
  public virtual void OnExpire(IEnemy target) { }

  public bool Update(IEnemy target, float deltaTime)
  {
    elapsed += deltaTime;
    OnTick(target, deltaTime);
    return elapsed >= duration;
  }
}
