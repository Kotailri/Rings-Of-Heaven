using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobEffect : MonoBehaviour
{
    private Vector3 defaultPos;

    private bool bobUp = true;
    public float bobDist;
    public float bobTime;

    public bool usesRectTransform;
    private bool positionLoaded = false;

    void Start()
    {
        StartCoroutine(LoadAfterDelay(0f));
    }

    public IEnumerator LoadAfterDelay(float t)
    {
        yield return new WaitForSeconds(t + Random.Range(0, 0.5f));
        if (usesRectTransform)
        {
            SetDefaultPos(GetComponent<RectTransform>().localPosition);
        }
        else
        {
            SetDefaultPos(transform.localPosition);
        }
        positionLoaded = true;

        if (Random.Range(0,1) == 0)
        {
            bobUp = !bobUp;
        }
    }

    public void SetDefaultPos(Vector3 dp)
    {
        defaultPos = dp;
    }

    void Update()
    {
        if (!positionLoaded)
        {
            return;
        }


        if (!LeanTween.isTweening(gameObject: gameObject))
        {
            
            bobUp = !bobUp;
            if (bobUp)
            {
                LeanTween.moveLocalY(gameObject, defaultPos.y + bobDist, bobTime);
            }
            else
            {
                LeanTween.moveLocalY(gameObject, defaultPos.y - bobDist, bobTime);
            }
        }
    }
}
