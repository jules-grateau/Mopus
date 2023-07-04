using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent") ]
public class GameEvent : ScriptableObject
{
    public List<GameEventListener> listeners = new List<GameEventListener>();

    public void Raise(Component component, object data)
    {
        listeners.ForEach(listener => listener.OnEventRaise(component, data));
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (listeners.Contains(listener)) return;
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener) {
        if (!listeners.Contains(listener)) return;
        listeners.Remove(listener);
    }
}
