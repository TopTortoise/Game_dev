using UnityEngine;
using System;
[CreateAssetMenu]
public class Statupgrade : Weaponupgrade
{
    public float damageBonus;
    public float fireRateBonus;
    public float range;

    public Statupgrade(float dmg, float firerate, float range ){
      upgradeID = Guid.NewGuid().ToString();
      damageBonus = dmg;
      fireRateBonus = firerate;
      this.range = range;
    }

    public override void Apply(IWeapon weapon)
    {
        weapon.stats.damage = Mathf.Max(this.damageBonus+weapon.stats.damage,0);
        weapon.stats.attackspeed = Mathf.Max(this.fireRateBonus+weapon.stats.attackspeed,0);
        weapon.stats.range = Mathf.Max(this.range+weapon.stats.range,0);
    }
}
