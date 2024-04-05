using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static GameObject GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player");
    }

    public static int BoolToInt(bool input)
    {
        return input ? 1 : 0;
    }

    public static bool IsWithinRadius(Vector2 center, Vector2 point, float radius)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(point.x - center.x, 2) + Mathf.Pow(point.y - center.y, 2));
        return distance <= radius;
    }

    public static bool IsOfTag(GameObject obj, Tags tag) 
    { 
        if (obj == null) return false;
        if (obj.TryGetComponent(out TagManager tagManager))
        {
            if (tagManager == null) return false;
            if (tagManager.IsOfTag(tag)) return true;
        }
        return false;
    }

    public static bool IsOfTagInList(GameObject obj, List<Tags> tagList)
    {
        if (obj == null) return false;
        if (obj.TryGetComponent(out TagManager tagManager))
        {
            if (tagManager == null) return false;
            foreach (Tags tag in tagList)
            {
                if (tagManager.IsOfTag(tag)) return true;
            }
        }
        return false;
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
