using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public abstract class IWeapon : MonoBehaviour
{
  public WeaponStats stats = new();
  public bool is_equipped = false;
  public InputAction AttackAction;
  public List<string> applied_upgrades;
  public List<Weaponupgrade> upgrades = new();
  public List<IWeaponEffect> effects = new();
  public abstract void Attack();
  public abstract void onEquip();


  public void equip(List<Weaponupgrade> upgrades)
  {//kill me
   //lovely comments here

    Debug.Log("equipping wiht as = " + stats.attackspeed);
    foreach (Weaponupgrade upgrade in upgrades)
    {
      if (applied_upgrades.Contains(upgrade.upgradeID))
      {
        continue;
      }
      applied_upgrades.Add(upgrade.upgradeID);
      upgrades.Add(upgrade);
      Debug.Log("upgrade applied");
      upgrade.Apply(this);
    }
    Debug.Log("equipping wiht as after aplly = " + stats.attackspeed);
    onEquip();
  }
  public void unequip()
  {
    onUnequip();
  }
  public abstract void onUnequip();
  public void OnAttack()
  {
    // shooting logic using currentStats
  }
  public void OnHit(IEnemy target)
  {

    var con = new HitContext
    {
      source = gameObject,
      target = target.gameObject,
      hitPoint = transform.position,
      baseDamage = stats.damage
    };

    con.target.GetComponent<IKillable>().hit(con.baseDamage);
    foreach (IWeaponEffect effect in effects)
    {
      if (effect is IOnHitEffect onhit)
      {
        Debug.Log("handlign onhit " + onhit);
        onhit.Apply(con);
      }
      else if (effect is StatusEffect stateffect)
      {
        IEnemy enemy = con.target.GetComponent<IEnemy>();
        if (enemy != null)
        {
          Debug.Log("handlign effects " + stateffect);
          enemy.ApplyEffect(stateffect);
        }
      }
    }


  }
}
