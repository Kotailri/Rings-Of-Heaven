using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HardModeSwitch : MonoBehaviour
{
    public Light2D lights;

    private bool hardmode = false;
    private bool onCooldown = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ring"))
        {
            if (!onCooldown)
                ToggleLights();
        }
    }

    private void ToggleLights()
    {
        hardmode = !hardmode;
        onCooldown = true;

        if (hardmode)
        {
            lights.pointLightOuterRadius = 10;
            lights.pointLightInnerRadius = 10;

            transform.rotation = Quaternion.Euler(0, 180, -138.817f);
            GetComponent<SpriteRenderer>().color = Color.green;
            AudioManager.instance.PlaySound("click");
        }
        else
        {
            lights.pointLightOuterRadius = 75;
            lights.pointLightInnerRadius = 0;

            transform.rotation = Quaternion.Euler(0, 0, -138.817f);
            GetComponent<SpriteRenderer>().color = Color.white;
            AudioManager.instance.PlaySound("click");
        }

        Utility.InvokeLambda(() =>
        {
            onCooldown = false;
        }, 0.5f);
    }
}
