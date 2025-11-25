using UnityEngine.InputSystem;
using UnityEngine;
public abstract class IWeapon: MonoBehaviour
{
  public float damage = 1f;
  public float attackspeed = 0.5f;
  public int sign = -1;//not good find better way to animate
  public InputAction AttackAction;
  public abstract void Attack();
}
