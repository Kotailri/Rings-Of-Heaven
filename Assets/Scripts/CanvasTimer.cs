using UnityEngine;
using TMPro;

public class CanvasTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float timer;
    private bool isPaused;

    private void Awake()
    {
        Global.timer = this;
    }

    private void Start()
    {
        timer = 0f;
        isPaused = true;
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (!isPaused)
        {
            timer += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        int milliseconds = Mathf.FloorToInt((timer * 1000) % 1000);

        string timerString = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        timerText.text = timerString;
    }

    public void PauseTimer(bool _isPaused)
    {
        isPaused = _isPaused;
    }

    public void ResetTimer()
    {
        timer = 0f;
        UpdateTimerDisplay();
    }
}
