public interface IWeaponEffect
{
    void OnAttack(IWeapon weapon);
    void OnHit(IWeapon weapon, IKillable target);
    void OnKill(IWeapon weapon, IKillable target);
}

