using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance;

    private Vector3 returnPosition;
    private bool hasReturnPosition = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called BEFORE entering loot room
    public void SaveReturnPosition()
    {
        
        returnPosition = transform.position + new Vector3(0f, -1f, 0f);

        Debug.Log("SAVE called at position: " + transform.position);
        hasReturnPosition = true;
    }

    // Called AFTER exiting loot room
    public void RestoreReturnPosition()
    {
        transform.position = returnPosition;
        Debug.Log("RESTORE (saved) position: " + transform.position);

        

        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      Debug.Log("hello PP");
        // Only auto-spawn when NOT returning
        if (scene.name != "SmallLootRoom" && scene.name != "LargeLootRoom")
        {
            GameObject spawn = GameObject.Find("PlayerSpawn");
            if (spawn != null && !hasReturnPosition)
            {
                transform.position = spawn.transform.position;
            }
            if(hasReturnPosition)
            {
                transform.position = returnPosition;
                hasReturnPosition = false; // consume it
            }
        }
        if (scene.name == "SmallLootRoom" || scene.name == "LargeLootRoom")
        {
            GameObject spawn = GameObject.Find("PlayerSpawn");
            transform.position = spawn.transform.position;
        }
    }
}




