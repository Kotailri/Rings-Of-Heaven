using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseControls : MonoBehaviour
{
    public void ArrowControlSelected()
    {
        Config.controlConfig = ControlConfig.Arrows;
        ChangeRooms();
    }

    public void WASDControlSelected()
    {
        Config.controlConfig = ControlConfig.WASD;
        ChangeRooms();
    }

    private void ChangeRooms()
    {
        SceneManager.LoadScene("PlatformerDemoScene");
    }
}
