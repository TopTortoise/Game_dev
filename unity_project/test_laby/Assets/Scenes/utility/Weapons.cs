using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public abstract class IWeapon : MonoBehaviour
{
  public WeaponStats stats;
  public bool is_equipped = false;
  public InputAction AttackAction;
  public List<string> applied_upgrades;
  public List<IWeaponEffect> effects = new();
  public abstract void Attack();
  public abstract void onEquip();
  public void equip(List<Weaponupgrade> upgrades){

    Debug.Log("equipping wiht as = " +stats.attackspeed);
    foreach(Weaponupgrade upgrade in upgrades){
      if(applied_upgrades.Contains(upgrade.upgradeID )){
        continue;
      }
      applied_upgrades.Add(upgrade.upgradeID);

      upgrade.Apply(this);
    }
    Debug.Log("equipping wiht as after aplly = " +stats.attackspeed);
    onEquip();
  }
  public void unequip(){
    onUnequip();
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
