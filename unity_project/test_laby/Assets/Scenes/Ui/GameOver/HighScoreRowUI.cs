using UnityEngine;
using TMPro;

public class HighScoreRowUI : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;


    public void Setup(int rank, string name, int score, int wave)
    {
        rankText.text = rank + ".";
        nameText.text = name;
        scoreText.text = score.ToString();

    }
}