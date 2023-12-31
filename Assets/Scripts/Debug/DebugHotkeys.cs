using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugHotkeys : MonoBehaviour
{
    private Controls controls;

    private void Awake()
    {
        controls = new Controls();
        controls.Gameplay.Restart.started += ctx => RestartGame();
        controls.Enable();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
