using System;
using System.Collections.Generic;

namespace Enigma
{
    /// <summary>
    /// A bag of arguments
    /// </summary>
    public class Arguments
    {

        private readonly Dictionary<string, object> _values;

        /// <summary>
        /// Creates a new instance of <see cref="Arguments"/>
        /// </summary>
        public Arguments()
        {
            _values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Sets a value in the list
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <param name="value">The value of the argument</param>
        public void Set(string name, object value)
        {
            _values[name] = value;
        }

        /// <summary>
        /// Gets a value in the list
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <returns>The argument value</returns>
        /// <exception cref="ArgumentNotFoundException">Thrown when the argument with the given name was not found</exception>
        public object Get(string name)
        {
            object value;
            if (!_values.TryGetValue(name, out value))
                throw new ArgumentNotFoundException(name);

            return value;
        }

        /// <summary>
        /// Tries to get a value in the list
        /// </summary>
        /// <param name="name">The name of the argument</param>
        /// <param name="value">The argument value</param>
        /// <returns><c>true</c> if the argument was found, otherwise false</returns>
        public bool TryGetValue(string name, out object value)
        {
            return _values.TryGetValue(name, out value);
        }

        /// <summary>
        /// Sets a value in the list
        /// </summary>
        /// <typeparam name="T">The type of the argument value</typeparam>
        /// <param name="value">The value of the argument</param>
        public void Set<T>(T value)
        {
            var name = typeof (T).FullName;
            _values[name] = value;;
        }

        /// <summary>
        /// Gets a value in the list
        /// </summary>
        /// <typeparam name="T">The type of the argument value</typeparam>
        /// <returns>The argument value</returns>
        /// <exception cref="ArgumentNotFoundException">Thrown when the argument with the given name was not found</exception>
        public T Get<T>()
        {
            var name = typeof(T).FullName;
            return (T) Get(name);
        }

        /// <summary>
        /// Tries to get a value in the list
        /// </summary>
        /// <typeparam name="T">The type of the argument value</typeparam>
        /// <param name="value">The argument value</param>
        /// <returns><c>true</c> if the argument was found, otherwise false</returns>
        public bool TryGetValue<T>(out T value)
        {
            var name = typeof(T).FullName;

            object untypedValue;
            if (!_values.TryGetValue(name, out untypedValue)) {
                value = default(T);
                return false;
            }

            value = (T) untypedValue;
            return true;
        }

    }
}
