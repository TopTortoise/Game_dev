using UnityEngine;

public class LootRoomManager : MonoBehaviour
{
  public GameObject[] enemies;
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
          Instantiate(Vase, pos.position, Quaternion.identity);
        }
        else
        {
          Instantiate(enemies[Random.Range(0, enemies.Length)], pos.position, Quaternion.identity);
        }
      }

    }
  }
}
