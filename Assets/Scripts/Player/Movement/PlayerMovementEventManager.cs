using System;
using System.Collections.Generic;

public enum PlayerMovementEvent
{
    OnDashStart,
    OnDashEnd,

    OnJump,
    OnDoubleJump,

    OnPlayerMove,
    OnPlayerStop,
    OnPlayerTurn,
    OnPlayerTurnGrounded
}

public static class PlayerMovementEventManager
{
    private static Dictionary<PlayerMovementEvent, Action<Dictionary<string, object>>> _eventDictionary = new();

    public static void StartListening(PlayerMovementEvent eventName, Action<Dictionary<string, object>> listener)
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

    public static void StopListening(PlayerMovementEvent eventName, Action<Dictionary<string, object>> listener)
    {
        Action<Dictionary<string, object>> thisEvent;
        if (_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
            _eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(PlayerMovementEvent eventName, Dictionary<string, object> message)
    {
        Action<Dictionary<string, object>> thisEvent;
        if (_eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(message);
        }
        else
        {
            Logger.PrintErr("Player Movement Event Manager triggered an event that caused an error.");
        }
    }
}
