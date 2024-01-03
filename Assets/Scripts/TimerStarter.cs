using UnityEngine;

public enum StartEnd
{
    Start,
    End
}

public class TimerStarter : MonoBehaviour
{
    public StartEnd startOrEnd;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (startOrEnd)
            {
                case StartEnd.Start:
                    StartGame();
                    break;

                case StartEnd.End:
                    EndGame();
                    break;
            }
            
        }
    }

    private void StartGame()
    {
        Global.timer.ResetTimer();
        Global.timer.PauseTimer(false);
        Managers.scoreManager.ResetScore();
        
    }

    private void EndGame()
    {
        Global.timer.PauseTimer(true);

        Managers.scoreManager.SaveBestTime();
        Managers.scoreManager.SaveHighScore();
    }
}
