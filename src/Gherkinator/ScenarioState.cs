using System;
using System.Collections.Generic;
using Gherkin.Ast;

namespace Gherkinator
{
    public class ScenarioState : IDisposable
    {
        readonly Dictionary<Type, object> data = new Dictionary<Type, object>();
        readonly Dictionary<Tuple<string, Type>, object> keyed = new Dictionary<Tuple<string, Type>, object>();
        readonly Dictionary<object, object> objects = new Dictionary<object, object>();
        readonly IEnumerable<Action<ScenarioState>> onDispose;

        public ScenarioState(Scenario scenario, IEnumerable<Action<ScenarioState>> onDispose = null)
        {
            Scenario = scenario;
            this.onDispose = onDispose ?? Array.Empty<Action<ScenarioState>>();
        }

        public Scenario Scenario { get; }

        public void Dispose()
        {
            foreach (var callback in onDispose)
            {
                callback(this);
            }
        }

        public virtual T Get<T>() => (T)data[typeof(T)];

        public virtual T Get<T>(string key) => (T)keyed[Tuple.Create(key.ToLowerInvariant(), typeof(T))];

        public virtual T Get<T>(object key) => (T)objects[key];

        public T GetOrSet<T>() where T : new()
            => data.TryGetValue(typeof(T), out var value) ? (T)value : Set(new T());

        public T GetOrSet<T>(string key) where T : new()
            => keyed.TryGetValue(Tuple.Create(key.ToLowerInvariant(), typeof(T)), out var value) ? (T)value : Set(key, new T());

        public T GetOrSet<T>(Func<T> factory)
            => data.TryGetValue(typeof(T), out var value) ? (T)value : Set(factory());

        public T GetOrSet<T>(string key, Func<T> factory)
            => keyed.TryGetValue(Tuple.Create(key.ToLowerInvariant(), typeof(T)), out var value) ? (T)value : Set(key, factory());

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

        /// <summary>
        /// Runs a verification action for the given state, optionally also disposing the 
        /// state (and invoking any OnDispose callbacks) if the verification is successful.
        /// </summary>
        /// <param name="action">The action that will verify the expected state.</param>
        /// <param name="disposeOnSuccess">Whether to also dispose the state when verification succeeds.</param>
        /// <remarks>
        /// State is always disposed (and all OnDispose callbacks invoked) when the verification fails 
        /// for whatever reason.
        /// </remarks>
        public ScenarioState Verify(Action<ScenarioState> action, bool disposeOnSuccess = true)
        {
            try
            {
                action(this);
                if (disposeOnSuccess)
                    Dispose();

                return this;
            }
            catch (Exception)
            {
                Dispose();
                // TODO: render a nice message with the failed scenario
                throw;
            }
        }
    }
}