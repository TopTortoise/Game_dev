using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

  public static GameManager Instance;

  public static string MainSceneName;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);

      MainSceneName = SceneManager.GetActiveScene().name;
      Debug.Log("GM says MainScene is " + MainSceneName);
    }
    else
    {
      Destroy(gameObject);
    }
  }






}
