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
        static ConcurrentDictionary<string, AsyncLocal<string>> state = new ConcurrentDictionary<string, AsyncLocal<string>>();

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData(string name, string data) =>
            state.GetOrAdd(name, _ => new AsyncLocal<string>()).Value = data;

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <param name="defaultValue">Optional default value if the given entry isn't found.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static object GetData(string name, string defaultValue = null) =>
            state.TryGetValue(name, out var data) ? data.Value : defaultValue;
    }
}