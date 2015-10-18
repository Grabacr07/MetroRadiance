// used from NotifyPropertyChangedGenerator

using System;
using System.Diagnostics;

namespace MetroRadiance.Chrome
{
    [Conditional("NEVER_USED_AT_RUNTIME")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class NotifyAttribute : Attribute,
        // option, you can customize naming convention rule
        NotifyAttribute.IPlain
    {
        // naming convention markers
        internal interface IPlain { }
        internal interface ILeadingUnderscore { }
        internal interface ITrailingUnderscore { }

        /// <summary>
        /// No option.
        /// </summary>
        public NotifyAttribute() { }

        /// <summary>
        /// Specify options.
        /// </summary>
        /// <param name="compareMethod">Comppare kind for raise property changed.</param>
        public NotifyAttribute(NotifyCompareMethod compareMethod) { }
    }

    [Conditional("NEVER_USED_AT_RUNTIME")]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class NonNotifyAttribute : Attribute
    {

    }

    /// <summary>
    /// Compare method in the generated SetProperty method.
    /// </summary>
    internal enum NotifyCompareMethod
    {
        /*
            - None: raises `PropertyChanged` at any time when the property set
            - ReferenceEquals: 
            - EqualityComparer: uses `EqualityComparer<T>.Default.Equals` to compare old and new values
        */
        /// <summary>
        /// Raises `PropertyChanged` at any time when the property set.
        /// </summary>
        None,

        /// <summary>
        /// Uses <see cref="object.ReferenceEquals(object, object)"/> to compare old and new values.
        /// </summary>
        ReferenceEquals,

        /// <summary>
        /// Uses <see cref="System.Collections.Generic.EqualityComparer{T}.Default"/> to compare old and new values.
        /// </summary>
        EqualityComparer,
    }
}