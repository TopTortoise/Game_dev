using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Waves/Wave")]
public class WaveData : ScriptableObject
{
    public List<GameObject> enemies;//change to enemy data for more modularity
    public float timeBetweenSpawns = 0.5f;
    public WaveData(float time, GameObject[] objects){
        timeBetweenSpawns = time;
        enemies = new List<GameObject>();
        enemies.AddRange(objects);

    }
}
