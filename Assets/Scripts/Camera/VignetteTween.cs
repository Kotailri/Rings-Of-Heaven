using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VignetteTween : MonoBehaviour
{
    public Q_Vignette_Single vignette;

    private float currentAlpha = 0.0f;
    private float alphaIncrement = 0.01f;
    private float alphaIncrementTime = 0.01f;

    private bool isTweening = false;

    private void Awake()
    {
        Global.vignetteTween = this;
    }

    public void SetVignetteDamage()
    {
        if (isTweening)
        {
            currentAlpha = 1;
            return;
        }

        Color c = GetVignetteColor();
        vignette.SetVignetteColour(new Color(c.r, c.g, c.b, 1));
        currentAlpha = 1;
        isTweening = true;

        StartCoroutine(IncrementAlpha());
    }

    private IEnumerator IncrementAlpha()
    {
        currentAlpha -= alphaIncrement;

        if (currentAlpha < 0)
        {
            isTweening = false;
            yield return null;
        }

        Color c = GetVignetteColor();
        vignette.SetVignetteColour(new Color(c.r, c.g, c.b, currentAlpha));

        yield return new WaitForSeconds(alphaIncrementTime);
        StartCoroutine(IncrementAlpha());
    }

    private Color GetVignetteColor()
    {
        return vignette.mainColor;
    }
}
