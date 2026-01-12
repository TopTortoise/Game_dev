using UnityEngine;
public abstract class Weaponupgrade
{

  public string upgradeName;
  public string upgradeID;
  public Sprite icon;

  public abstract void Apply(IWeapon weapon);
}
