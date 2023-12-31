using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugHotkeys : MonoBehaviour
{
    public void RestartGame(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerPrefs.DeleteAll();
            AudioManager.instance.PlaySound("click");
        }
    }
}
