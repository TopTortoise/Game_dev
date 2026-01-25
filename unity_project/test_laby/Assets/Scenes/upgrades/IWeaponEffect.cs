using UnityEngine;
[System.Serializable]
public abstract class IWeaponEffect : ScriptableObject
{

    // void OnAttack(IWeapon weapon);
    // void OnHit(IWeapon weapon, IKillable target);
    // void OnKill(IWeapon weapon, IKillable target);
    public abstract string GetDescription();
    
}

