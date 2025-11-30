using UnityEngine.InputSystem;
using UnityEngine;
public abstract class IWeapon: MonoBehaviour
{
  public float damage = 1f;
  public float attackspeed = 0.5f;
  public bool is_equipped= false;
  public InputAction AttackAction;
  public abstract void Attack();
  public abstract void equip();
  public abstract void unequip();
}
