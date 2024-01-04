using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum StartEnd
{
    Start,
    End
}

public class GameObjectSave
{
    public GameObject savedObject;
    public Vector2 position;

    public GameObjectSave(GameObject gm, Vector2 pos)
    {
        savedObject = gm;
        position = pos;
    }
}

public class TimerStarter : MonoBehaviour
{
    public StartEnd startOrEnd;
    private List<GameObjectSave> savedGameObjects = new();

    private void Start()
    {
        foreach (GameObject gm in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            savedGameObjects.Add(new GameObjectSave(gm, gm.transform.position));
        }

        foreach (GameObject gm in GameObject.FindGameObjectsWithTag("ScoreAdder"))
        {
            savedGameObjects.Add(new GameObjectSave(gm, gm.transform.position));
        }
    }

    private void ClearObjects()
    {
        foreach (GameObjectSave gameObjectSave in savedGameObjects)
        {
            gameObjectSave.savedObject.transform.position = Config.poolPosition;
        }
    }

    private void LoadObjects()
    {
        foreach (GameObjectSave gameObjectSave in savedGameObjects)
        {
            gameObjectSave.savedObject.transform.position = gameObjectSave.position;
        }
    }

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

        LoadObjects();
    }

    private void EndGame()
    {
        Global.timer.PauseTimer(true);

        Managers.scoreManager.SaveBestTime();
        Managers.scoreManager.SaveHighScore();

        ClearObjects();
    }
}
