using UnityEngine;

public enum StartEnd
{
    Start,
    End
}

public class TimerStarter : MonoBehaviour
{
    public StartEnd startOrEnd;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (startOrEnd)
            {
                case StartEnd.Start:
                    if (!activated)
                        StartGame();
                    break;

                case StartEnd.End:
                    if (activated)
                        EndGame();
                    break;
            }
            
        }
    }

    private void Start()
    {
        // load best score and time
        // update board
    }

    private void StartGame()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.1f);
        Global.timer.ResetTimer();
        Global.timer.PauseTimer(false);
        activated = true;
    }

    private void EndGame()
    {
        GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.1f);
        Global.timer.PauseTimer(true);
        activated = false;

        Managers.scoreManager.SaveBestTime();
        Managers.scoreManager.SaveHighScore();
        // update board
    }
}
