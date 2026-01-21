using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance;

    private Vector3 returnPosition;
    private bool hasReturnPosition = false;

    //Checking SpawnPoint Collisions
    [SerializeField] private Vector2 playerCapsuleSize = new Vector2(2f, 4f);
    [SerializeField] private CapsuleDirection2D capsuleDirection = CapsuleDirection2D.Vertical;
    [SerializeField] private LayerMask blockingLayer;



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
    public void SaveReturnPosition(Collision2D collision)
    {
        Transform portal = collision.transform;

        foreach (Transform probe in portal)
        {
            // Check if a player-sized capsule would overlap anything
            Collider2D hit = Physics2D.OverlapCapsule(
                probe.position,
                playerCapsuleSize,
                capsuleDirection,
                0f,
                blockingLayer
            );

            if (hit == null)
            {
                returnPosition = probe.position;
                hasReturnPosition = true;

                Debug.Log("Saved return position at: " + probe.name);
                return;
            }
        }

        // Fallback (should almost never happen)
        returnPosition = portal.position;
        hasReturnPosition = true;

        Debug.LogWarning("All portal spawn capsules blocked.");
    }



    // Called AFTER exiting loot room
    public void RestoreReturnPosition()
    {
        transform.position = returnPosition;
        Debug.Log("RESTORE (saved) position: " + transform.position);

        

        
    }

    public bool HasReturnPosition()
    {
        return hasReturnPosition;

    }

    public void ResetReturnPosition()
    {
        hasReturnPosition = false;
    }


    

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      Debug.Log("hello PP");
        // Only auto-spawn when NOT returning
        if (scene.name != "SmallLootRoom" && scene.name != "LargeLootRoom")
        {
            GameObject spawn = GameObject.Find("PlayerSpawn");
            if (spawn != null )
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




