using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
public class Enemy_Manager : MonoBehaviour
{

  public GameObject[] enemy_prefabs;
  public GameObject[] Boss_prefabs;
  public GameObject[] portal_prefabs;
  public GameObject[] trap_prefabs;
  public List<GameObject> items;
  public GameObject vase;
  private ArrayList end_maze_positions;
  private ArrayList in_maze_positions;
  public float vase_spawnrate = 0.75f;
  public float enemy_spawnrate = 0.5f;
  public float portal_spawnrate = 1.0f;
  public List<WaveData> waves;
  public int current_wave_index = 0;


    /*
    void Start()
    {
        if (GameState.Instance == null)
        {
            Debug.LogError("GameState missing!");
            return;
        }

        GameState.Instance.OnCycleEnded += OnCycleEnded;
        Debug.Log("Enemy_Manager subscribed to OnCycleEnded");
    }

  void OnDestroy()
  {
      if (GameState.Instance != null)
          GameState.Instance.OnCycleEnded -= OnCycleEnded;
  }


    
    void OnCycleEnded()
    {
        StartCoroutine(spawnWave());
    }*/

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void loadprefabs()
  {

    string dir = "Assets/Scenes/weapons";
    string[] files = Directory.GetFiles(dir, "*.prefab", SearchOption.TopDirectoryOnly);
    foreach (string file in files)
    {
      Debug.Log(file);
      GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(file);
      if (prefab != null)
      {
        // Debug.Log(prefab);
        items.Add(prefab);
        // Instantiate(prefab);   
      }
    }
    // Cursor.visible = false;
  }

  public void SetPositions(ArrayList end_maze, ArrayList in_maze)
  {

    end_maze_positions = end_maze;
    in_maze_positions = in_maze;
  }
  public ArrayList getPositions()
  {
    return in_maze_positions;
  }

  public void setup()
  {
    loadprefabs();
    fill_maze();


    waves = new List<WaveData>();
    GameObject[] wave1 = { enemy_prefabs[1], enemy_prefabs[1], enemy_prefabs[1],
      enemy_prefabs[0], enemy_prefabs[2], enemy_prefabs[0], enemy_prefabs[2],
      enemy_prefabs[0], enemy_prefabs[1], enemy_prefabs[2] };
    waves.Add(new WaveData(0.5f, wave1));
  }

  void fill_maze()
  {

    foreach (Vector3Int position in in_maze_positions)
    {

      go.Add(Instantiate(trap_prefabs[Random.Range(0, trap_prefabs.Length)], position, Quaternion.identity));
    }


    foreach (Vector3Int position in end_maze_positions)
    {
      float value = Random.value;
      bool item_spawn = false;
      // bool enemy_spawn = false;
      if (value < vase_spawnrate)
      {
        spawnItem(position);
        item_spawn = true;
      }
      if (value < enemy_spawnrate)
      {
        place_enemy(position);
        // enemy_spawn = true;

      }
      if (!item_spawn && value < portal_spawnrate)
      {
        place_portal(position);
      }



    }
  }


  ArrayList go = new();
  public void free_everyhtig()
  {
    foreach (GameObject obj in go)
    {
      Destroy(obj);
    }
  }

  void place_portal(Vector3Int position)
  {
    if (position.y > 12)
    {

      go.Add(Instantiate(portal_prefabs[Random.Range(0, portal_prefabs.Length)], position, Quaternion.identity));
    }
  }


  void place_enemy(Vector3Int position)
  {
    go.Add(Instantiate(enemy_prefabs[Random.Range(0, enemy_prefabs.Length)], position, Quaternion.identity));
  }

/*
  
    Vector3[] spawn_pos =
  {
      new Vector3(250, -20, 0),
      new Vector3(250, -30, 0),
      new Vector3(250, -40, 0),
      new Vector3(250, -50, 0)
  };


  public IEnumerator spawnWave()
  {

      if (waves == null || waves.Count == 0) {
        Debug.Log("waves == null || waves.Count == 0");
        yield break;
      }
      if (current_wave_index >= waves.Count) {
        Debug.Log("current_wave_index >= waves.Count");
        yield break;
      }
      
      WaveData curr_wave = waves[current_wave_index];

      foreach (var enemy in curr_wave.enemies)
      {
          if (enemy == null) continue;

          Instantiate(enemy,
              spawn_pos[Random.Range(0, spawn_pos.Length)],
              Quaternion.identity);

          yield return new WaitForSeconds(curr_wave.timeBetweenSpawns);
      }

      yield return new WaitForSeconds(5f);

      if (Boss_prefabs.Length > 0 && Boss_prefabs[0] != null)
      {
          Instantiate(Boss_prefabs[0],
              spawn_pos[Random.Range(0, spawn_pos.Length)],
              Quaternion.identity);
      }
  }
*/


  void spawnItem(Vector3Int pos)
  {
    // Debug.Log("random value is: " + rand);
    Vector3 spawnPos = pos + new Vector3(1f, 1f, 0);
    float distance_to_start = Vector3.Distance(spawnPos, Vector3.zero);

    GameObject inst = Instantiate(vase, spawnPos, Quaternion.identity);
    go.Add(inst);
    //map distanc to rarity, 
    //TODO: items need rarity and then chosen randomly from the value
    //also some cases should be empty 
    int mapped = Mathf.FloorToInt(Mathf.Lerp(0f, 2.5f, Mathf.InverseLerp(0f, 250f, distance_to_start)) + Random.value);
    GameObject item = items[mapped];

    if (mapped == 0 && Random.value < 0.5)
    {

      inst.GetComponent<Vase>().item = null;
    }
    else
    {

      inst.GetComponent<Vase>().item = item;
    }
    // Debug.Log($"Vase spawned at {spawnPos} with Distance {distance_to_start} and index {mapped} with item {inst.GetComponent<Vase>().item}");
  }


}
