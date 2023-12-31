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
            if (activated)
                return;

            activated = true;

            switch (startOrEnd)
            {
                case StartEnd.Start:
                    GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.1f);
                    Global.timer.PauseTimer(false);
                    break;
                case StartEnd.End:
                    GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.1f);
                    Global.timer.PauseTimer(true);
                    break;
            }
            
        }
    }
}
