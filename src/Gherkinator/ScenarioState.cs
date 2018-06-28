using System;
using System.Collections.Generic;

namespace Gherkinator
{
    public class ScenarioState
    {
        readonly Dictionary<Type, object> data = new Dictionary<Type, object>();
        readonly Dictionary<Tuple<string, Type>, object> keyed = new Dictionary<Tuple<string, Type>, object>();

        public T Get<T>() => (T)data[typeof(T)];

        public T Get<T>(string key) => (T)keyed[Tuple.Create(key, typeof(T))];

        public void Set<T>(T value) => data[typeof(T)] = value;

        public void Set<T>(string key, T value) => keyed[Tuple.Create(key, typeof(T))] = value;
    }
}
