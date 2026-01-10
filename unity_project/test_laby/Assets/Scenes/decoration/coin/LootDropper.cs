using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LootItem
{
    public string name;        // Nur zur Übersicht im Inspector
    public GameObject prefab;  // Das Item, das spawnen soll
    public float weight;       // Je höher das Gewicht, desto wahrscheinlicher (z.B. Gold = 80, Rubin = 5)
}

public class LootDropper : MonoBehaviour
{
    public List<LootItem> lootTable = new List<LootItem>();
    
    [Header("Drop Chance")]
    [Range(0, 100)]
    public float chanceToDropAnything = 100f; // Ob der Gegner überhaupt IRGENDWAS droppt

    public void DropLoot()
    {
        // 1. Grundsätzliche Prüfung: Droppt überhaupt etwas?
        if (Random.Range(0f, 100f) > chanceToDropAnything) return;

        // 2. Gesamtgewicht berechnen
        float totalWeight = 0;
        foreach (LootItem item in lootTable)
        {
            totalWeight += item.weight;
        }

        // 3. Zufallszahl zwischen 0 und Gesamtgewicht würfeln
        float roll = Random.Range(0, totalWeight);
        float currentSum = 0;

        // 4. Den Gewinner ermitteln
        foreach (LootItem item in lootTable)
        {
            currentSum += item.weight;
            if (roll <= currentSum)
            {
                SpawnItem(item.prefab);
                return; // Methode sofort beenden, damit nur EIN Item spawnt
            }
        }
    }

    void SpawnItem(GameObject prefab)
    {
        if (prefab == null) return;

        GameObject droppedObj = Instantiate(prefab, transform.position, Quaternion.identity);
        
        // Der physikalische "Herausflieg"-Effekt
        Rigidbody2D rb = droppedObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            rb.AddForce(randomDir * Random.Range(4f, 8f), ForceMode2D.Impulse);
        }
    }
}