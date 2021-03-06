﻿using System;
using System.Collections.Generic;

namespace Gherkinator
{
    public class ScenarioContext
    {
        readonly Dictionary<Type, object> data = new Dictionary<Type, object>();
        readonly Dictionary<Tuple<string, Type>, object> keyed = new Dictionary<Tuple<string, Type>, object>();
        readonly Dictionary<object, object> objects = new Dictionary<object, object>();

        public ScenarioContext() { }

        public virtual T Get<T>() 
            => data.ContainsKey(typeof(T))
            ? (T)data[typeof(T)]
            : throw new KeyNotFoundException($"Key not found: <{typeof(T).Name}>()");

        public virtual T Get<T>(string key) 
            => keyed.ContainsKey(Tuple.Create(key.ToLowerInvariant(), typeof(T))) 
            ? (T)keyed[Tuple.Create(key.ToLowerInvariant(), typeof(T))] 
            : throw new KeyNotFoundException($"Key not found: <{typeof(T).Name}>({key})");

        public virtual T Get<T>(object key) 
            => objects.ContainsKey(key) 
            ? (T)objects[key] 
            : throw new KeyNotFoundException($"Key not found: {key}");

        public virtual bool TryGet<T>(out T value)
        {
            value = default;
            if (data.TryGetValue(typeof(T), out var obj))
            {
                value = (T)obj;
                return true;
            }
            return false;
        }

        public virtual bool TryGet<T>(string key, out T value)
        {
            value = default;
            if (keyed.TryGetValue(Tuple.Create(key.ToLowerInvariant(), typeof(T)), out var obj))
            {
                value = (T)obj;
                return true;
            }
            return false;
        }

        public virtual bool TryGet<T>(object key, out T value)
        {
            value = default;
            if (objects.TryGetValue(key, out var obj))
            {
                value = (T)obj;
                return true;
            }
            return false;
        }

        public T GetOrSet<T>() where T : new()
            => data.TryGetValue(typeof(T), out var value) ? (T)value : Set(new T());

        public T GetOrSet<T>(string key) where T : new()
            => keyed.TryGetValue(Tuple.Create(key.ToLowerInvariant(), typeof(T)), out var value) ? (T)value : Set(key, new T());

        public T GetOrSet<T>(Func<T> factory)
            => data.TryGetValue(typeof(T), out var value) ? (T)value : Set(factory());

        public T GetOrSet<T>(string key, Func<T> factory)
            => keyed.TryGetValue(Tuple.Create(key.ToLowerInvariant(), typeof(T)), out var value) ? (T)value : Set(key, factory());

        public T GetOrSet<T>(object key, Func<T> factory)
            => objects.TryGetValue(key, out var value) ? (T)value : Set(key, factory());

        public virtual T Set<T>(T value)
        {
            data[typeof(T)] = value;
            return value;
        }

        public virtual T Set<T>(string key, T value)
        {
            keyed[Tuple.Create(key.ToLowerInvariant(), typeof(T))] = value;
            return value;
        }

        public virtual T Set<T>(object key, T value)
        {
            objects[key] = value;
            return value;
        }
    }
}