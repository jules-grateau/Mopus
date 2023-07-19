using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.CustomEvents
{
    public class GameEvent 
    {
        private event Action _action;

        public void Invoke()
        {
            _action.Invoke();
        }

        public void AddListener(Action listener)
        {
            _action += listener;
        }

        public void RemoveListener(Action listener)
        {
            _action -= listener;
        }
    }

    public class GameEvent<T>
    {
        private event Action<T> _action;

        public void Invoke(T param)
        {
            _action.Invoke(param);
        }

        public void AddListener(Action<T> listener)
        {
            _action += listener;
        }

        public void RemoveListener(Action<T> listener)
        {
            _action -= listener;
        }
    }

    public class GameEvent<T,U>
    {
        private event Action<T,U> _action;

        public void Invoke(T param, U param2)
        {
            _action.Invoke(param, param2);
        }

        public void AddListener(Action<T,U> listener)
        {
            _action += listener;
        }

        public void RemoveListener(Action<T,U> listener)
        {
            _action -= listener;
        }
    }
}