using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static int BoolToInt(bool input)
    {
        return input ? 1 : 0;
    }

    public static bool IsWithinRadius(Vector2 center, Vector2 point, float radius)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(point.x - center.x, 2) + Mathf.Pow(point.y - center.y, 2));
        return distance <= radius;
    }

    public static void InvokeLambda(Action action, float time)
    {
        MonoBehaviour script = new GameObject().AddComponent<Invoker>();
        script.StartCoroutine(script.GetComponent<Invoker>().InvokeCoroutine(action, time));
    }

    private class Invoker : MonoBehaviour
    {
        public IEnumerator InvokeCoroutine(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            if (isActiveAndEnabled)
            {
                action.Invoke();
                Destroy(gameObject);
            }
        }
    }
    
}
