public interface IWeaponEffect
{
    void OnAttack(IWeapon weapon);
    void OnHit(IWeapon weapon, IKillable target);
}

