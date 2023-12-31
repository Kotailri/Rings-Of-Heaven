using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    public int currentScore = 0;
    public TextMeshProUGUI scoreText;

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
}
