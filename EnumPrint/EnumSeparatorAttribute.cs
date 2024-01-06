using System;

namespace EnumPrint
{
    /// <summary>
    /// Specifies the separator that is used for creating the string representations of <see cref="Enum"/> value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class EnumSeparatorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumSeparatorAttribute"/> class, and specifies the separator.
        /// </summary>
        /// <param name="separator">The separator that is used for creating the string representations of <see cref="Enum"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="separator"/> is <c>null</c>.</exception>
        public EnumSeparatorAttribute(string separator)
        {
            Separator = separator ?? throw new ArgumentNullException(nameof(separator));
        }

        /// <summary>
        /// Get the separator.
        /// </summary>
        public string Separator { get; }
    }
}
