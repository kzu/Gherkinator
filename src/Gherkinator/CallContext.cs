using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Gherkinator
{
    /// <summary>
    /// Provides a way to set contextual data that flows with the call and 
    /// async context of a test or invocation.
    /// </summary>
    public static class CallContext
    {
        static ConcurrentDictionary<Tuple<string, Type>, AsyncLocal<object>> state = new ConcurrentDictionary<Tuple<string, Type>, AsyncLocal<object>>();
        static ConcurrentDictionary<Type, AsyncLocal<object>> typed = new ConcurrentDictionary<Type, AsyncLocal<object>>();

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <param name="setInitialValue">Optional default value if the given entry isn't found.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static T GetData<T>(string name, T setInitialValue = default)
        {
            var local = state.GetOrAdd(Tuple.Create(name, typeof(T)), _ => new AsyncLocal<object> { Value = setInitialValue });
            if (object.Equals(local.Value, default(T)))
                local.Value = setInitialValue;

            return (T)local.Value;
        }

        /// <summary>
        /// Retrieves an object with the specified type from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="setInitialValue">Optional default value if the given entry isn't found.</param>
        /// <returns>The object in the call context with the specified type, or <paramref name="setInitialValue"/> if not found.</returns>
        public static T GetData<T>(T setInitialValue = default)
        {
            var local = typed.GetOrAdd(typeof(T), _ => new AsyncLocal<object> { Value = setInitialValue });
            if (object.Equals(local.Value, default(T)))
                local.Value = setInitialValue;

            return (T)local.Value;
        }

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData<T>(string name, T data) =>
            state.GetOrAdd(Tuple.Create(name, typeof(T)), _ => new AsyncLocal<object>()).Value = data;

        /// <summary>
        /// Stores a given object.
        /// </summary>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData<T>(T data) =>
            typed.GetOrAdd(typeof(T), _ => new AsyncLocal<object>()).Value = data;
    }
}