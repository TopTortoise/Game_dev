using UnityEngine.InputSystem;
using UnityEngine;
public abstract class IWeapon: MonoBehaviour
{
  public float damage;
  public float attackspeed;
  public int sign;//not good find better way to animate
  public InputAction AttackAction;
  public abstract void Attack();
}
