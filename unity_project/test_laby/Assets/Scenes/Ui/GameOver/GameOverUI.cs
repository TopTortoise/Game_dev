using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("Stats Display")]
    public TextMeshProUGUI wavesText;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI bossesText;
    public TextMeshProUGUI upgradesText;
    public TextMeshProUGUI totalScoreText;

    [Header("Input")]
    public TMP_InputField nameInput; 
    public Button submitButton;
    public GameObject inputPanel; 
    public GameObject highScoreListPanel; 
    public TextMeshProUGUI highScoreListText;

    private int calculatedScore;

    void Start()
    {
        ShowStats();
        
        // Input Setup: Max 3 Zeichen, alles Großbuchstaben
        nameInput.characterLimit = 3;
        nameInput.onValidateInput += delegate(string input, int charIndex, char addedChar) { return char.ToUpper(addedChar); };
        
        submitButton.onClick.AddListener(SubmitScore);
    }

    void ShowStats()
    {
        // Werte aus GameState holen
        int waves = GameState.Instance.nrWavesDefeated;
        int enemies = GameState.Instance.nrEnemiesDefeated;
        int bosses = GameState.Instance.nrBossesDefeated;
        int upgrades = GameState.Instance.nrTempleUpgrades;
        
        // Score berechnen
        calculatedScore = GameState.Instance.CalculateTotalScore();

        // UI Updaten
        wavesText.text = $"Waves Survived: {waves}";
        enemiesText.text = $"Enemies Defeated: {enemies}";
        bossesText.text = $"Bosses Slain: {bosses}";
        upgradesText.text = $"Temple Upgrades: {upgrades}";
        
        // Großes Score Display
        totalScoreText.text = $"TOTAL SCORE: {calculatedScore}";
    }

    public void SubmitScore()
    {
        string playerName = nameInput.text;

        if (string.IsNullOrEmpty(playerName))
            playerName = "UNK"; // Default Name falls leer

        // Score speichern
        HighScoreManager.Instance.AddScore(playerName, calculatedScore, GameState.Instance.nrWavesDefeated);

        // UI umschalten (Input ausblenden, Button deaktivieren)
        inputPanel.SetActive(false);
        
        ShowLeaderboard();
        
    
    }

    void ShowLeaderboard()
    {
        if(highScoreListPanel != null) 
        {
            highScoreListPanel.SetActive(true);
            var scores = HighScoreManager.Instance.GetHighScores();
            
            string listText = "TOP SCORES\n\n";
            int rank = 1;
            foreach(var entry in scores)
            {
                
                listText += $"{rank}. {entry.name} ..... {entry.score}\n";
                rank++;
            }
            highScoreListText.text = listText;
        }
    }
    
    public void GoToMainMenu()
    {
         GameState.Instance.ResetGameState();
         SceneManager.LoadScene("TitleScene");
    }
}