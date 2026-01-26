using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


public class GameState : MonoBehaviour
{
  public static GameState Instance;

  public float templeHealth = 100;
  public float currentTempleHealth = 100;
  // ---- Clock state ----
  public int timeRemaining;
  public bool isCountingDown;
  public bool warningStarted;

  public bool enemyWaveActive;

  // ---- Progression events ----
  public event Action OnCycleEnded;
  public event Action OnCycleStarted;

  public event Action OnClockPaused;
  public event Action OnClockResumed;


  // ---- High Score Game Stats (For Death Page)
  public int nrWavesDefeated;
  public int nrBossesDefeated;
  public int nrEnemiesDefeated;
  public int nrTempleUpgrades;

  public int levelHealth = 1;
  public int levelUltDamage = 1;
  public int levelUltCooldown = 1;


  // ---- Enemy Wave State
  public float SpawnInterval = 2.5f;
  public int EnemiesPerWave = 10;

  public int nrBosses = 0;

  // ---- Day Length
  public int DayDuration = 300;


  // ---- state correction for nrEnemyWaves
  private bool enemyWaveStarted;


  //---- to keep tabs on entered lootrooms
  //---- Difficulty set in Title scene
  void ApplyDifficultySettings()
  {

    if (GameData.selectedDifficulty == Difficulty.TeacherMode)
    {
      Debug.Log("Teacher Mode activated: Temple is very strong!");

      templeHealth = 10000f;
    }
    else
    {
      Debug.Log("Normal Mode: Good Luck!");
      templeHealth = 100f;
    }

    currentTempleHealth = templeHealth;
  }
  void UpdateEnemyWaveDifficulty()
  {
    if (enemyWaveStarted)
    {
      nrBosses += 1;
    }

    SpawnInterval -= UnityEngine.Random.Range(0.05f, 0.1f); ;
    EnemiesPerWave += UnityEngine.Random.Range(2, 7); ;
  }

  public void SetTempleHealth(float amount)
  {
    templeHealth = amount;
  }

  public void SetCurrentTempleHealth(float amount)
  {
    nrTempleUpgrades++;
    currentTempleHealth = amount;
  }

  void UpdateDayDuration()
  {
    DayDuration += 60;
  }

  //for GameOverManager
  public void ResetGameState()
  {
    enemyWaveStarted = false;
    SpawnInterval = 2.5f;
    EnemiesPerWave = 10;
    nrBosses = 0;
    DayDuration = 300;
    nrWavesDefeated = 0;
    nrBossesDefeated = 0;
    nrEnemiesDefeated = 0;
    nrTempleUpgrades = 0;
    CurrencyManager.Instance.ResetCoins();
    currentTempleHealth = templeHealth;

  }

  void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);
    nrBossesDefeated = 0;
    nrWavesDefeated = 0; //start with 0 (first day starts with 0)

    ApplyDifficultySettings();
    enemyWaveStarted = false;
  }

  public void StartNewCycle(int duration)
  {
    GetComponent<maze_gen>().seed = UnityEngine.Random.Range(0,int.MaxValue);
    GetComponent<maze_gen>().reset();

    Temple temple = FindFirstObjectByType<Temple>();
    if (temple != null)
    {
      temple.ResetTemple();
    }
    else
    {
      Debug.LogWarning("Temple not Found!");
    }
    enemyWaveActive = false;
    AudioManager.Instance.ChangeMusic(AudioManager.SoundType.Music_Day);
    UpdateEnemyWaveDifficulty();
    if (enemyWaveStarted)
    {
      nrWavesDefeated++;
      enemyWaveStarted = false;
    }
    timeRemaining = duration;
    isCountingDown = true;
    warningStarted = false;

    OnCycleStarted?.Invoke();
  }

  public void EndCycle()
  {
    AudioManager.Instance.ChangeMusic(AudioManager.SoundType.Music_Defend_The_Temple);
    UpdateDayDuration();
    isCountingDown = false;
    enemyWaveActive = true;
    OnCycleEnded?.Invoke();
    enemyWaveStarted = true;
    Debug.Log("OnCycleEnded invoked");
  }


  public int CalculateTotalScore()
  {
    int score = 0;


    score += nrEnemiesDefeated * 10;
    score += nrBossesDefeated * 500;
    score += nrWavesDefeated * 100;
    score += nrTempleUpgrades * 50;

    return score;
  }

  public void PauseClock()
  {

    OnClockPaused?.Invoke();
  }

  public void ResumeClock()
  {

    OnClockResumed?.Invoke();
  }
}



