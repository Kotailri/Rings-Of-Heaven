using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    public int currentScore = 0;
    public TextMeshProUGUI scoreText;

    [Space(10f)]
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI bestTimeText;

    private int totalPossibleScore = 0;

    private void Awake()
    {
        Managers.scoreManager = this;
    }

    private void Start()
    {
        totalPossibleScore = GameObject.FindGameObjectsWithTag("ScoreAdder").Length;
        totalPossibleScore += GameObject.FindGameObjectsWithTag("Enemy").Length;
        UpdateScoreUI();

        if (GetHighScore() != 0)
        {
            bestScoreText.text = GetHighScore().ToString();
        }

        if (GetBestTime() != int.MaxValue)
        {
            bestTimeText.text = Global.timer.FormatTime(Global.timer.GetTime());
        }
    }

    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = "Score: " + currentScore + " / " + totalPossibleScore;
    }

    // SAVE LOAD SCORE
    private const string HIGH_SCORE_KEY = "HighScore";
    private const string BEST_TIME_KEY = "BestTime";

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
        int currentBestTime = PlayerPrefs.GetInt(BEST_TIME_KEY, int.MaxValue);
        int time = Global.timer.GetTime();

        if (time < currentBestTime)
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
        return PlayerPrefs.GetInt(BEST_TIME_KEY, int.MaxValue);
    }
}
