using System;
using System.Collections.Generic;

public static class EventStrings
{
    public static readonly string PLAYER_HEALED  = "PLAYER_HEALED";
    public static readonly string PLAYER_DAMAGED = "PLAYER_DAMAGED";
    public static readonly string PLAYER_HIT     = "PLAYER_HIT";
    public static readonly string PLAYER_DEATH   = "PLAYER_DIED";

    public static readonly string PLAYER_KNOCKED_BACK     = "PLAYER_KNOCKED_BACK";
    public static readonly string PLAYER_DASH_INTERRUPTED = "PLAYER_DASH_INTERRUPT";
    public static readonly string PLAYER_DASH_RESET       = "PLAYER_DASH_RESET";
}

public static class EventManager
{
    private static Dictionary<string, Action<Dictionary<string, object>>> _eventDictionary = new();

    public static void StartListening(string eventName, Action<Dictionary<string, object>> listener)
    {
        Action<Dictionary<string, object>> thisEvent;

        if (_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += listener;
            _eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            _eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action<Dictionary<string, object>> listener)
    {
        Action<Dictionary<string, object>> thisEvent;
        if (_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            _eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName, Dictionary<string, object> message)
    {
        Action<Dictionary<string, object>> thisEvent;
        if (_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(message);
        }
        else
        {
            Logger.PrintErr("Generic Event Manager triggered an event that caused an error.");
        }
    }
}

// ========== EXAMPLE ============
/*

EventManager.TriggerEvent("gameOver", null);
EventManager.TriggerEvent("addReward", new Dictionary<string, object> {
    { "name", "candy" },
    { "amount", 5 }  
});

void OnEnable()
{
    EventManager.StartListening("addCoins", OnAddCoins);
}

void OnDisable()
{
    EventManager.StopListening("addCoins", OnAddCoins);
}

void OnAddCoins(Dictionary<string, object> message)
{
    var amount = (int)message["amount"];
    coins += amount;
}
*/
