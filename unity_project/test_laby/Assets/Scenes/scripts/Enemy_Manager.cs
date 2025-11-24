using UnityEngine;

using System.Collections;
public class Enemy_Manager : MonoBehaviour
{

    public GameObject[] prefabs;
    private ArrayList positions;
    public Vector3Int start_pos = new Vector3Int(16, 16, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Cursor.visible = false;
        GameObject prefab = Resources.Load<GameObject>("Scenes/enemy");
        if (prefab != null)
        {
          Debug.Log("it Worked!!");
          
            // Instantiate(prefab, new Vector3(0, 1, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Prefab not found in Resources/Prefabs folder!");
        }
    }

    public void SetPositions(ArrayList pos)
    {

        positions = pos;
    }
    public ArrayList getPositions()
    {
        return positions;
    }

    public void setup()
    {
        place_enemies();
        spawnItems();
    }

    void place_enemies()
    {
        foreach (Vector3Int position in positions)
        {
            Instantiate(prefabs[Random.Range(0,prefabs.Length)], position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    void spawnItems()
    {
        

        
        foreach (Vector3Int pos in positions)
        {
            if (Random.value < 0.2f) // 20% Chance auf Item in Sackgasse
            {
                Vector3 spawnPos = pos + new Vector3(0.5f, 0.5f, 0);
                Instantiate(prefabs[1], spawnPos, Quaternion.identity);
                Debug.Log($"Item spawned at {spawnPos}");
            }
        }
            
        
    }
}
