using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Weapon Effects/Effect Pool")]
public class EffectPool : ScriptableObject
{
    public List<IWeaponEffect> effects;
    
    public IWeaponEffect GetRandomEffect(){

      return effects[Random.Range(0,effects.Count-1)];
    }
    
}
