using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
public class GameManager : MonoBehaviour
{

  public static GameManager Instance;

  public static string MainSceneName;
  public static ExplosionVisualizer ep;
  public Vector3 SpawnPoint;
  public Dictionary<EntityId, (Vector3, float)> Torchpoint;
  public GameObject Torch;
  private void Awake()
  {
    Torchpoint = new();
    ep = new ExplosionVisualizer();
    Time.timeScale = 1f;
    if (Instance == null)
    {
      Instance = this;

      MainSceneName = SceneManager.GetActiveScene().name;
      FindAnyObjectByType<ghost>().gameObject.transform.position = SpawnPoint;

    }
    else
    {
      Destroy(gameObject);
    }
    SceneManager.sceneLoaded += OnSceneLoaded;
    DontDestroyOnLoad(gameObject);
  }



  public List<Vector3> lootrooms;
  private int counter = 0;

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    if (scene.name != "MainScene") return;
    if (scene.name != "MainScene" && counter > 0) return;
    GetComponent<maze_gen>().Start();
    GameObject[] small_loot = GameObject.FindGameObjectsWithTag("Enter Loot Room Portal");
    GameObject[] big_loot = GameObject.FindGameObjectsWithTag("Enter Large Loot Room Portal");
    //TODO might be inefficient??
    foreach (GameObject room in small_loot)
    {
      if (lootrooms.Contains(room.transform.position))
      {
        Destroy(room);
      }
    }

    var oldTorches = Torchpoint.ToList(); // snapshot
    Torchpoint.Clear();

    foreach (var torch in oldTorches)
    {
      var pos = torch.Value.Item1;
      var health = torch.Value.Item2;

      GameObject t = Instantiate(Torch, pos, Quaternion.identity);
      // t.GetComponent<TorchTurret>().hp.set_hp(health);

      Torchpoint.Add(t.GetEntityId(), (pos, health));
    }
    foreach (GameObject room in big_loot)
    {
      if (lootrooms.Contains(room.transform.position))
      {
        Destroy(room);
      }
    }
    counter++;
  }
}
