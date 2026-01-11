using UnityEngine;
[CreateAssetMenu]
public class Statupgrade : Weaponupgrade
{
    public float damageBonus;
    public float fireRateBonus;
    public float range;

    public override void Apply(IWeapon weapon)
    {
        weapon.stats.damage += this.damageBonus;
        weapon.stats.attackspeed += this.fireRateBonus;
        weapon.stats.range += this.range;
    }
}
