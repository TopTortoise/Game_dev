using UnityEngine;
using System;
using System.Text;
[CreateAssetMenu]
public class Statupgrade : Weaponupgrade
{
  public float damageBonus;
  public float fireRateBonus;
  public float range;

  public Statupgrade(float dmg, float firerate, float range)
  {
    upgradeID = Guid.NewGuid().ToString();
    damageBonus = dmg;
    fireRateBonus = firerate;
    this.range = range;
  }

    public override string GetDescription()
    {
        StringBuilder sb = new StringBuilder();
        
        if (damageBonus > 0) sb.Append($"+{damageBonus} Dmg ");
        if (fireRateBonus > 0) sb.Append($"+{fireRateBonus} Spd ");
        if (range > 0) sb.Append($"+{range} Rng");

        if (sb.Length == 0) return "Stat Boost"; 
        return sb.ToString();
    }
  public override void Apply(IWeapon weapon)
  {
    Debug.Log("apllying this sheet "+fireRateBonus+" "+damageBonus);
    weapon.stats.damage = Mathf.Max(this.damageBonus + weapon.stats.damage, 0);
    weapon.stats.attackspeed = Mathf.Max(this.fireRateBonus + weapon.stats.attackspeed, 0);
    weapon.stats.range = Mathf.Max(this.range + weapon.stats.range, 0);
  }
}
