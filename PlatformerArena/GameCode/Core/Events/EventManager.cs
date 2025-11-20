using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class EventManager
    {
        private static EventManager _instance;
        private readonly Dictionary<Type, List<(object target, Delegate action)>> _subscribers = new();
        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventManager();
                    return _instance;
                }
                else return _instance;
            }
        }

        public void Subscribe<T>(Action<T> callback)
        {
            Type t = typeof(T);
            if (!_subscribers.ContainsKey(t))
                _subscribers[t] = new();

            _subscribers[t].Add((callback.Target, callback));
        }
        public void Unsubscribe<T>(Action<T> callback)
        {
            Type t = typeof(T);
            if (_subscribers.TryGetValue(t, out var list))
            {
                // Удаляем все совпадающие делегаты
                list.RemoveAll(sub => sub.target == callback.Target && sub.action.Method == callback.Method);

                // Если после удаления список пуст — убираем сам тип из словаря
                if (list.Count == 0)
                    _subscribers.Remove(t);
            }
        }
        public void Trigger<T>(T e)
        {
            Type t = typeof(T);
            if (_subscribers.TryGetValue(t, out var list))
            {
                foreach (var (target, action) in list.ToList())
                    ((Action<T>)action)(e);
            }
        }

    }

}


