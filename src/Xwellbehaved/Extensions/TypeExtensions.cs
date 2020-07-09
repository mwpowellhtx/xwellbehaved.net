using System;
using System.Reflection;

namespace Xwellbehaved
{
    // #4 MWP 2020-07-09 05:45:48 PM / Background methods support parameter default values.
    /// <summary>
    /// Provides a set of useful <see cref="Type"/> extension methods.
    /// </summary>
    internal static class DefaultValue
    {
        /// <summary>
        /// Returns the Default value for <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <see cref="GetDefault(Type)"/>
        public static T GetDefault<T>() => (T) typeof(T).GetDefault();

        /// <summary>
        /// Returns the Default Value for a given <paramref name="type"/>.
        /// <para/>
        /// If a null <see cref="Type"/>, a reference <see cref="Type"/>, or a <see cref="Void"/>
        /// <see cref="Type"/>is supplied, this method always returns <c>null</c>. If a value
        /// <see cref="Type"/> is supplied which is not publicly visible or which contains generic
        /// parameters, this method will fail with an exception.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to get the Default value.</param>
        /// <returns>The default value for <paramref name="type"/></returns>
        /// <example>
        /// To use this method in its native, non-extension form, make a call like:
        /// <code>
        ///     object Default = DefaultValue.GetDefault(someType);
        /// </code>
        /// To use this method in its <see cref="Type"/>-extension form, make a call like:
        /// <code>
        ///     object Default = someType.GetDefault();
        /// </code>
        /// </example>
        /// <seealso cref="GetDefault{T}"/>
        public static object GetDefault(this Type type)
        {
            // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null.
            if (type == null || !type.IsValueType || type == typeof(void))
            {
                return null;
            }

            var nl = Environment.NewLine;
            var currentMethod = MethodInfo.GetCurrentMethod();

            // If the supplied Type has generic parameters, its default value cannot be determined.
            if (type.ContainsGenericParameters)
            {
                throw new ArgumentException(
                    $"{{{currentMethod}}} Error:{nl}{nl}The supplied value type <{type.FullName}>"
                        + " contains generic parameters, so the default value cannot be retrieved")
                    .RedressDefaultException(type);
            }

            /* If the Type is a primitive type, or if it is another publicly visible value type,
             * i.e. struct or enum, return a default instance of the value type. */
            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        $"{{{currentMethod}}} Error:{nl}{nl}The {nameof(Activator)}.{nameof(Activator.CreateInstance)}"
                            + $" method could not create a default instance of the supplied value type <{type.FullName}>"
                            + $" (Inner Exception message: \"{ex.Message}\")", ex)
                        .RedressDefaultException(type)
                        .RedressDefaultException(ex);
                }
            }

            // Fail with exception.
            throw new ArgumentException(
                $"{{{currentMethod}}} Error:{nl}{nl}The supplied value type <{type.FullName}>"
                    + "is not a publicly visible type, so the default value cannot be retrieved")
                .RedressDefaultException(type);
        }

        private static TException RedressDefaultException<TException>(this TException ex, Type type)
            where TException : Exception
        {

            ex.Data.Add(nameof(type), type);
            return ex;
        }

        private static TException RedressDefaultException<TException>(this TException ex, Exception innerException)
            where TException : Exception
        {

            ex.Data.Add(nameof(innerException), innerException);
            return ex;
        }
    }
}
