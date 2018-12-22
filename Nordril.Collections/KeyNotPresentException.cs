using System;

namespace Nordril.HedgingEngine.Logic.Collections
{
    /// <summary>
    /// Indicates that a string-key was not present when an attempt was made to
    /// delete it from a collection.
    /// </summary>
    public class KeyNotPresentException : Exception
    {
        /// <summary>
        /// The key or its string-form.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Creates a new <see cref="KeyNotPresentException"/>.
        /// </summary>
        /// <param name="key">The value of the duplicate key.</param>
        public KeyNotPresentException(string key) : base()
        {
            Key = key;
        }

        /// <summary>
        /// Creates a new <see cref="KeyNotPresentException"/>
        /// </summary>
        public KeyNotPresentException()
        {
        }

        /// <summary>
        /// Creates a new <see cref="KeyNotPresentException"/> with a custom error message and an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public KeyNotPresentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
