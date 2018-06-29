using System;
using System.Collections.Generic;

namespace Gherkinator
{
    public class ScenarioState
    {
        readonly Dictionary<Type, object> data = new Dictionary<Type, object>();
        readonly Dictionary<Tuple<string, Type>, object> keyed = new Dictionary<Tuple<string, Type>, object>();

        public virtual T Get<T>() => (T)data[typeof(T)];

        public virtual T Get<T>(string key) => (T)keyed[Tuple.Create(key, typeof(T))];

        public T GetOrSet<T>() where T : new() 
            => data.TryGetValue(typeof(T), out var value) ? (T)value : Set(new T());

        public T GetOrSet<T>(string key) where T : new()
            => keyed.TryGetValue(Tuple.Create(key, typeof(T)), out var value) ? (T)value : Set(key, new T());

        public T GetOrSet<T>(Func<ScenarioState, T> factory)
            => data.TryGetValue(typeof(T), out var value) ? (T)value : Set(factory(this));

        public T GetOrSet<T>(string key, Func<ScenarioState, T> factory)
            => keyed.TryGetValue(Tuple.Create(key, typeof(T)), out var value) ? (T)value : Set(key, factory(this));

        public virtual T Set<T>(T value)
        {
            data[typeof(T)] = value;
            return value;
        }

        public virtual T Set<T>(string key, T value)
        {
            keyed[Tuple.Create(key, typeof(T))] = value;
            return value;
        }
    }
}
