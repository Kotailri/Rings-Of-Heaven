using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    public int currentScore = 0;
    public TextMeshProUGUI scoreText;

    [Space(10f)]
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI bestTimeText;

    [Space(5f)]
    public TextMeshProUGUI enemiesKilledText;

    private int totalPossibleScore = 0;

    private void Awake()
    {
        Managers.scoreManager = this;
    }

    private void Start()
    {
        totalPossibleScore = GameObject.FindGameObjectsWithTag("ScoreAdder").Length;
        UpdateScoreUI();

        if (GetHighScore() != 0)
        {
            bestScoreText.text = GetHighScore().ToString();
        }

        if (GetBestTime() != 0)
        {
            bestTimeText.text = Global.timer.FormatTime(GetBestTime());
        }

        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        UpdateEnemyKilledUI();
    }

    private void OnEnable()
    {
        EventManager.StartListening(EventStrings.SCORE_ADDED, OnAddScore);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventStrings.SCORE_ADDED, OnAddScore);
    }

    private void OnAddScore(Dictionary<string, object> msg)
    {
        AddScore((int)msg["score"]);
    }

    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Coins: " + currentScore + " / " + totalPossibleScore;
    }

    // SAVE LOAD SCORE
    private const string HIGH_SCORE_KEY = "HighScore2";
    private const string BEST_TIME_KEY = "BestTime2";

    public void SaveHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        if (currentScore > currentHighScore)
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, currentScore);
            PlayerPrefs.Save();

            bestScoreText.text = currentScore.ToString();
        }
    }

    public void SaveBestTime()
    {
        int currentBestTime = PlayerPrefs.GetInt(BEST_TIME_KEY, 0);
        int time = Global.timer.GetTime();
        if (time < currentBestTime || currentBestTime == 0)
        {
            PlayerPrefs.SetInt(BEST_TIME_KEY, time);
            PlayerPrefs.Save();

            bestTimeText.text = Global.timer.FormatTime(time);
        }
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    public int GetBestTime()
    {
        return PlayerPrefs.GetInt(BEST_TIME_KEY, 0);
    }

    // ENEMIES KILLED
    private int enemiesKilled = 0;
    private int totalEnemies = 0;

    public void ResetEnemiesKilled()
    {
        enemiesKilled = 0;
        UpdateEnemyKilledUI();
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;
        UpdateEnemyKilledUI();
    }

    private void UpdateEnemyKilledUI()
    {
        enemiesKilledText.text = "Enemies Killed: " + enemiesKilled + " / " + totalEnemies;
    }
}
