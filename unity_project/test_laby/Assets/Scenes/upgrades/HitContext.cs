using UnityEngine;
public struct HitContext
{
  public GameObject source;   // weapon / player
  public GameObject target;
  public Vector3 hitPoint;
  public Vector3 hitNormal;
  public float baseDamage;
}
