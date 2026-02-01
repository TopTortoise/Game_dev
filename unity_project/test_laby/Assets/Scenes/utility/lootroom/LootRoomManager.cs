using UnityEngine;

public class LootRoomManager : MonoBehaviour
{
  public GameObject[] enemies;
  public GameObject[] items;
  public EffectPool effects;

  public GameObject Vase;
  Transform[] positions;
  public float vase_spawnrate;
  public float enemy_spawnrate;
  public float spwanrate;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {

    positions = GetComponentsInChildren<Transform>();
    Debug.Log(positions);
    place_stuff();
  }

  void place_stuff()
  {
    foreach (Transform pos in positions)
    {
      float value = Random.value;
      if (value <= spwanrate)
      {

        value = Random.value;
        if (value <= vase_spawnrate)
        {

          GameObject vase = Instantiate(Vase, pos.position, Quaternion.identity);

          GameObject item = items[Random.Range(0, items.Length)];

          GameObject weapon = Instantiate(item, new Vector3Int(-1000, -1000), Quaternion.identity);

          vase.GetComponent<Vase>().item = weapon;
          // weapon.GetComponent<IWeapon>().effects.Add()
          if (weapon.GetComponent<IWeapon>().stats.rarity > 1)
          {
            weapon.GetComponent<IWeapon>().effects.Add(effects.GetRandomEffect());
            // inst.GetComponent<Vase>().weapon_upgrade = weapon;
          }
          if (weapon.GetComponent<IWeapon>().stats.rarity > 2)
          {

            weapon.GetComponent<IWeapon>().effects.Add(effects.GetRandomEffect());
          }

          Statupgrade upgrade = new Statupgrade(0.2f + Random.value, -(Random.value/3), 0f);


          vase.GetComponent<Vase>().weapon_upgrade = upgrade;
        }
        else
        {
          Instantiate(enemies[Random.Range(0, enemies.Length)], pos.position, Quaternion.identity);
        }
      }

    }
  }
}
