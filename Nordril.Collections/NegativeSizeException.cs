using System;

namespace Nordril.Collections
{
    /// <summary>
    /// Indicates that an invalid, negative size was passed as a parameter into a container.
    /// </summary>
    public class NegativeSizeException : Exception
    {
        /// <summary>
        /// The key or its string-form.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Creates a new <see cref="NegativeSizeException"/>.
        /// </summary>
        /// <param name="key">The value of the duplicate key.</param>
        public NegativeSizeException(string key) : base()
        {
            Key = key;
        }

        /// <summary>
        /// Creates a new <see cref="NegativeSizeException"/>
        /// </summary>
        public NegativeSizeException()
        {
        }

        /// <summary>
        /// Creates a new <see cref="NegativeSizeException"/> with a custom error message and an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NegativeSizeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
