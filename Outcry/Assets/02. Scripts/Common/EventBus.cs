using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus
{
    private static Dictionary<string, Action<object>> listeners = new();

    public static void Subscribe(string eventKey, Action<object> callback)
    {
        if (!listeners.ContainsKey(eventKey))
        {
            listeners[eventKey] = delegate { };
        }
        listeners[eventKey] += callback;
    }

    public static void Unsubscribe(string eventKey, Action<object> callback)
    {
        if (listeners.ContainsKey(eventKey))
        {
            listeners[eventKey] -= callback;

        }
    }

    public static void Publish(string eventKey, object data = null)
    {
        if (listeners.TryGetValue(eventKey, out var action))
        {
            action.Invoke(data);
        }
    }
}