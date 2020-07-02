using System;
using System.Collections.Generic;
using System.Linq;

namespace Xunit
{
    using Xunit.Sdk;

    // TODO: TBD: these are a couple of strong candidates for addition to the xunit.fluently.assert family...
    // TODO: TBD: in the meantime, we just add them here...
    /// <summary>
    /// Here are some prototype extension methods which could potentially serve as the
    /// basis to inform additional efforts in the <c>xunit.fluently.assert</c> family
    /// of Xunit extension methods.
    /// </summary>
    internal static class FluentXunitExtensionMethods
    {
        /// <summary>
        /// Asserts whether the <paramref name="actual"/> Is Assignable to
        /// <typeparamref name="TExpected"/>. Short of expecting <c>value</c> be that Type at
        /// compile time.
        /// </summary>
        /// <typeparam name="TExpected"></typeparam>
        /// <param name="actual"></param>
        public static object AssertIsAssignableTo<TExpected>(this object actual)
            => actual.AssertTrue(x => (x is null && (typeof(TExpected).IsClass
                || typeof(TExpected) == typeof(object))) || x is TExpected);

        /// <summary>
        /// Returns whether <paramref name="actual"/> Is Assignable To the
        /// <paramref name="expected"/> type, accounting for <c>null</c> cases.
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        public static object AssertIsAssignableTo(this object actual, Type expected)
        {
            Type GetActualType()
            {
                var actualType = actual?.GetType();

                if (actual == null)
                {
                    const string name = nameof(actual);
                    throw new ArgumentNullException($"{name} is null", name);
                }

                return actualType;
            }

            expected.AssertTrue(
                x => (actual is null && (expected.IsClass || expected == typeof(object)))
                    || x.IsAssignableFrom(GetActualType())
            );

            return actual;
        }

        /// <summary>
        /// Not quite the same thing as Asserting whether Equal, or whether
        /// <paramref name="actual"/> contains any or all of the <paramref name="expected"/> taken
        /// individually, we want to know whether the <paramref name="expected"/> appears
        /// anywhere in the range of <paramref name="actual"/> as an intact Sequence. Uses the
        /// given <paramref name="equalityComparer"/> to determine element equality.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="equalityComparer"></param>
        /// <param name="expected"></param>
        /// <returns>The range of <paramref name="values"/> following successful assertion.</returns>
        /// <see cref="IEqualityComparer{T}"/>
        /// <see cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource}, IEqualityComparer{TSource})"/>
        public static IEnumerable<T> AssertContainsSequence<T>(this IEnumerable<T> actual, IEqualityComparer<T> equalityComparer, params T[] expected)
        {
            static int Min(int x, int y) => Math.Min(x, y);

            var contains = false;
            var actualCount = actual.Count();

            for (var i = 0; !contains && i < actualCount; i++)
            {
                // Take as many of the Values as possible out to the Sequence Length.
                var lengthToTake = Min(actualCount - i, expected.Length);
                contains = actual.Skip(i).Take(lengthToTake).SequenceEqual(expected, equalityComparer);
            }

            if (!contains)
            {
                throw new ContainsException(expected, actual);
            }

            return actual;
        }

        /// <summary>
        /// Not quite the same thing as Asserting whether Equal, or whether
        /// <paramref name="actual"/> contains any or all of the <paramref name="expected"/> taken
        /// individually, we want to know whether the <paramref name="expected"/> appears anywhere
        /// in the range of <paramref name="actual"/> as an intact Sequence. The only difference
        /// here is that we provide the default <see cref="IEqualityComparer{T}"/>,
        /// <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns>The range of <paramref name="actual"/> following successful assertion.</returns>
        /// <see cref="IEqualityComparer{T}"/>
        /// <see cref="EqualityComparer{T}.Default"/>
        /// <see cref="AssertContainsSequence{T}(IEnumerable{T}, IEqualityComparer{T}, T[])"/>
        public static IEnumerable<T> AssertContainsSequence<T>(this IEnumerable<T> actual, params T[] expected)
            => actual.AssertContainsSequence(EqualityComparer<T>.Default, expected);
    }
}
