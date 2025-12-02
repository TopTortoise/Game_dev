using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
public class Enemy_Manager : MonoBehaviour
{

    public GameObject[] prefabs;
    public List<GameObject> items;
    public GameObject vase;
    private ArrayList positions;
    public Vector3Int start_pos = new Vector3Int(16, 16, 0);
    public float vase_spawnrate = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void loadprefabs()
    {

        string dir = "Assets/weapons";
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
        loadprefabs();
        place_enemies();
        spawnItems();
    }

    void place_enemies()
    {
        foreach (Vector3Int position in positions)
        {
            Instantiate(prefabs[Random.Range(0, prefabs.Length)], position, Quaternion.identity);
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
          float rand = Random.value;
          Debug.Log("random value is: "+ rand);
            if (rand < vase_spawnrate) // 20% Chance auf Item in Sackgasse
            {
                Vector3 spawnPos = pos + new Vector3(1f, 1f, 0);
                
                float distance_to_start = Vector3.Distance(spawnPos, Vector3.zero);

                GameObject inst = Instantiate(vase, spawnPos, Quaternion.identity);
                
                //map distanc to rarity, 
                //TODO: items need rarity and then chosen randomly from the value
                //also some cases should be empty 
                int mapped = Mathf.FloorToInt(Mathf.Lerp(0f, 2.5f, Mathf.InverseLerp(0f, 250f, distance_to_start)) +  Random.value);
                GameObject item = items[mapped];
                if(mapped == 0 && Random.value < 0.5){
                  
                  inst.GetComponent<Vase>().item = null;
                }else{
                  
                  inst.GetComponent<Vase>().item = item;
                }
                Debug.Log($"Vase spawned at {spawnPos} with Distance {distance_to_start} and index {mapped} with item {inst.GetComponent<Vase>().item}");
            }
        }


    }
}
