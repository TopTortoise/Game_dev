using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootItem
{
    public string name;        
    public GameObject prefab;  
    public float weight;       
}

public class LootDropper : MonoBehaviour
{
    public List<LootItem> lootTable = new List<LootItem>();
    
    [Header("Drop Chance")]
    [Range(0, 100)]
    public float chanceToDropAnything = 100f; 

    public void DropLoot()
    {
       
        if (Random.Range(0f, 100f) > chanceToDropAnything) return;

        
        float totalWeight = 0;
        foreach (LootItem item in lootTable)
        {
            totalWeight += item.weight;
        }

      
        float roll = Random.Range(0, totalWeight);
        float currentSum = 0;

        
        foreach (LootItem item in lootTable)
        {
            currentSum += item.weight;
            if (roll <= currentSum)
            {
                SpawnItem(item.prefab);
                return; 
            }
        }
    }

    void SpawnItem(GameObject prefab)
    {
        if (prefab == null) return;

        GameObject droppedObj = Instantiate(prefab, transform.position, Quaternion.identity);
        
       
        Rigidbody2D rb = droppedObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            rb.AddForce(randomDir * Random.Range(4f, 8f), ForceMode2D.Impulse);
        }
    }
}