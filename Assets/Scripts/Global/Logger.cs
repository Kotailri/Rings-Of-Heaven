using UnityEngine;

public static class Logger
{
    public static bool PrintWarnEnabled = true;
    public static bool PrintDebugEnabled = true;
    public static bool PrintErrorEnabled = true;

    public static void PrintCol(string print, string hexkey)
    {
        Debug.Log("<color=#" + hexkey + ">" + print + "</color>");
    }

    public static void PrintCol(string print, Color color)
    {
        PrintCol(print, ColorUtility.ToHtmlStringRGB(color));
    }

    public static void PrintWarn(string print)
    {
        if (PrintWarnEnabled)
        {
            PrintCol(print, "FFA500");
        }
    }

    public static void PrintErr(string print)
    {
        if (!PrintErrorEnabled) 
        {
            PrintCol(print, "FF0000");
            Time.timeScale = 0;
        }
    }

    public static void PrintDebug(string print)
    {
        if (PrintDebugEnabled)
        {
            PrintCol(print, "00FFFF");
        }
    }
}
