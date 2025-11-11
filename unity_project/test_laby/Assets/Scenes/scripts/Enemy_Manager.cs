using UnityEngine;

using System.Collections;
public class Enemy_Manager : MonoBehaviour
{

    public GameObject[] prefabs;
    private ArrayList positions;
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

    public void setup()
    {
        place_enemies();
    }

    void place_enemies()
    {
        foreach (Vector3Int position in positions)
        {
            Instantiate(prefabs[0], position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
