using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject scorePanel;       // Das Panel mit "Game Over", Stats und Name-Input
    public GameObject leaderboardPanel; // Das Panel mit der Liste

    [Header("Leaderboard List Setup")]
    public Transform scoreContainer;    // Das Objekt mit der Vertical Layout Group (Wo die Zeilen reinkommen)
    public GameObject rowPrefab;        // Dein Zeilen-Prefab (ScoreRowTemplate)

    [Header("Button Switch")]
    public TextMeshProUGUI switchButtonText;

    [Header("Stats Display")]
    public TextMeshProUGUI wavesText;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI bossesText;
    public TextMeshProUGUI upgradesText;
    public TextMeshProUGUI totalScoreText;

    [Header("Input")]
    public TMP_InputField nameInput; 
    public Button submitButton;
    public GameObject inputPanel; // Das kleine Feld wo man den Namen eingibt

    private int calculatedScore;
    void OnEnable()
    {
        scorePanel.SetActive(true);
        leaderboardPanel.SetActive(false);
        if(inputPanel != null) inputPanel.SetActive(true);
        if(switchButtonText != null) switchButtonText.text = "Leaderboard";

       
        RefreshData();
    }
    void Start()
    {
        // Setup für Input und Buttons
        if(nameInput != null) {
            nameInput.characterLimit = 4;
            nameInput.onValidateInput += delegate(string input, int charIndex, char addedChar) { return char.ToUpper(addedChar); };
        }
        if(submitButton != null) submitButton.onClick.AddListener(SubmitScore);
    }
void RefreshData()
    {
        if (GameState.Instance == null) return;

        // Daten holen
        int waves = GameState.Instance.nrWavesDefeated;
        int enemies = GameState.Instance.nrEnemiesDefeated;
        int bosses = GameState.Instance.nrBossesDefeated;
        int upgrades = GameState.Instance.nrTempleUpgrades;

        calculatedScore = GameState.Instance.CalculateTotalScore();
        
        // Alternativ, wenn du die Funktion im GameState hast:
        // calculatedScore = GameState.Instance.CalculateTotalScore();

        // UI Texte setzen
        if(wavesText) wavesText.text = $"Waves: {waves}";
        if(enemiesText) enemiesText.text = $"Enemies: {enemies}";
        if(bossesText) bossesText.text = $"Bosses: {bosses}";
        if(upgradesText) upgradesText.text = $"Upgrades: {upgrades}";
        if(totalScoreText) totalScoreText.text = $"SCORE: {calculatedScore}";

        Debug.Log("Game Over Stats geladen. Score: " + calculatedScore);
    }

    public void SubmitScore()
    {
        string playerName = nameInput.text;

        if (string.IsNullOrEmpty(playerName))
            playerName = "UNK"; 


        HighScoreManager.Instance.AddScore(playerName, calculatedScore, GameState.Instance.nrWavesDefeated);


        if(inputPanel != null) inputPanel.SetActive(false);
        

        OpenLeaderboard(); 
    }

    public void TogglePanels()
    {
        bool isLeaderboardOpen = leaderboardPanel.activeSelf;

        if (isLeaderboardOpen)
        {

            leaderboardPanel.SetActive(false);
            scorePanel.SetActive(true);
            switchButtonText.text = "Leaderboard";
        }
        else
        {
        
            OpenLeaderboard();
        }
    }

    void OpenLeaderboard()
    {
        scorePanel.SetActive(false);
        leaderboardPanel.SetActive(true);
        switchButtonText.text = "Back"; 

        UpdateLeaderboardList(); // Das ist die neue Magie!
    }


    void UpdateLeaderboardList()
    {

        foreach (Transform child in scoreContainer)
        {
            Destroy(child.gameObject);
        }


        var scores = HighScoreManager.Instance.GetHighScores();

        int rank = 1;
        foreach (var entry in scores)
        {
            GameObject newRow = Instantiate(rowPrefab, scoreContainer);
            

            HighScoreRowUI rowScript = newRow.GetComponent<HighScoreRowUI>();
            if (rowScript != null)
            {
                rowScript.Setup(rank, entry.name, entry.score, entry.waveReached);
            }
            
           
            if (rank == 1) 
            {
                 Image rowBg = newRow.GetComponent<Image>();
                 if(rowBg != null) rowBg.color = new Color(1f, 0.84f, 0f, 0.3f); 
            }

            rank++;
        }
    }
    
    public void GoToMainMenu()
    {
         if(GameState.Instance != null) GameState.Instance.ResetGameState();
         SceneManager.LoadScene("TitleScene");
    }
}