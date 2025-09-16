using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observable<T>
{
    private T value;
    private readonly string eventKey;

    public Observable(string key, T initValue = default)
    {
        eventKey = key;
        value = initValue;
    }

    public T Value
    {
        get => value;
        set
        {
            if (!Equals(this.value, value))
            {
                this.value = value;
                EventBus.Publish(eventKey, this.value);
            }
        }
    }
}
