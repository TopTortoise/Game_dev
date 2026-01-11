using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public abstract class IWeapon : MonoBehaviour
{
  public WeaponStats stats;
  public bool is_equipped = false;
  public InputAction AttackAction;
  public List<IWeaponEffect> effects = new();
  public abstract void Attack();
  public abstract void onEquip();
  public void equip(List<Weaponupgrade> upgrades){
    foreach(Weaponupgrade upgrade in upgrades){
      upgrade.Apply(this);
    }
    onEquip();
  }
  public abstract void onUnequip();
  public void OnAttack()
  {
    foreach (var effect in effects)
      effect.OnAttack(this);

    // shooting logic using currentStats
  }
  public void OnHit(IKillable target)
  {
    foreach (var effect in effects)
      effect.OnHit(this, target);
  }
}
