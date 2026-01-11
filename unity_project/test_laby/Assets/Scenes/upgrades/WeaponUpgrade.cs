using UnityEngine;
public abstract class Weaponupgrade
{

  public string upgradeName;
  public Sprite icon;

  public abstract void Apply(IWeapon weapon);
}
