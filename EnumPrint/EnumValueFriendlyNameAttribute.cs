using System;

namespace EnumPrint
{
    /// <summary>
    ///  Specifies the friendly name of an <see cref="Enum"/> value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class EnumValueFriendlyNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueFriendlyNameAttribute"/> class, and specifies the friendly name.
        /// </summary>
        /// <param name="printName">Friendly name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="printName"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="printName"/> is empty or contains only of white-space.</exception>
        public EnumValueFriendlyNameAttribute(string printName)
        {
            if (printName == null)
            {
                throw new ArgumentNullException(nameof(printName));
            }
            if (string.IsNullOrWhiteSpace(printName))
            {
                throw new ArgumentException("printName is empty or contains only of white-space.", nameof(printName));
            }
            Name = printName;
        }

        /// <summary>
        /// Get the friendly name.
        /// </summary>
        public string Name { get; }
    }
}
