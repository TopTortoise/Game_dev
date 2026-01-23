using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

  public static GameManager Instance;

  public static string MainSceneName;

  public Vector3 SpawnPoint;

  private void Awake()
  {
    Time.timeScale = 1f;
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);

      MainSceneName = SceneManager.GetActiveScene().name;
      Debug.Log("GM says MainScene is " + MainSceneName);
      FindAnyObjectByType<ghost>().gameObject.transform.position = SpawnPoint;

    }
    else
    {
      Destroy(gameObject);
    }
    SceneManager.sceneLoaded += OnSceneLoaded;
  }



  public List<Vector3> lootrooms;
  private int counter = 0;

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    if (scene.name != MainSceneName && counter > 0) return;
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
