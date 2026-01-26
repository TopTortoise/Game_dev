using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Wichtig für Sortierung

[System.Serializable]
public class HighScoreEntry
{
    public string name;
    public int score;
    public int waveReached; // Optional: Um auch die Welle in der Liste anzuzeigen
}

[System.Serializable]
public class HighScoreList
{
    public List<HighScoreEntry> entries = new List<HighScoreEntry>();
}

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;
    private string saveKey = "ArcadeHighScores";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(this);
    }

    public void AddScore(string playerName, int totalScore, int wave)
    {
        HighScoreList list = LoadScores();


        HighScoreEntry newEntry = new HighScoreEntry { name = playerName, score = totalScore, waveReached = wave };
        list.entries.Add(newEntry);

  
        list.entries = list.entries.OrderByDescending(x => x.score).Take(10).ToList();

        SaveScores(list);
    }

    public List<HighScoreEntry> GetHighScores()
    {
        return LoadScores().entries;
    }

    private void SaveScores(HighScoreList list)
    {
        string json = JsonUtility.ToJson(list);
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
    }

    private HighScoreList LoadScores()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string json = PlayerPrefs.GetString(saveKey);
            return JsonUtility.FromJson<HighScoreList>(json);
        }
        return new HighScoreList();
    }
}