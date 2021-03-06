using System;
using System.Collections.Generic;
using System.Linq;

namespace SkyrimAnimationChecker
{
    public static class EnumerableExtensions
    {
        public static void For<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            for (int i = 0; i < source.Count(); i++)
            {
                action(i, source.ElementAt(i));
            }
        }
        public static IEnumerable<U> For<T, U>(this IEnumerable<T> source, Func<int, T, U> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            U[] output = new U[source.Count()];
            for (int i = 0; i < source.Count(); i++)
            {
                output[i] = action(i, source.ElementAt(i));
            }
            return output.AsEnumerable();
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (T item in source)
            {
                action(item);
            }
        }
        //public static T[] ForEach<T>(this T[] source, Func<T, T> action)
        //{
        //    if (source == null) throw new ArgumentNullException("source");
        //    if (action == null) throw new ArgumentNullException("action");

        //    T[] output = new T[source.Length];
        //    for (int i = 0; i < source.Count(); i++)
        //    {
        //        output[i] = action(source[i]);
        //    }
        //    return output;
        //}
        public static U[] ForEach<T, U>(this T[] source, Func<T, U> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            U[] output = new U[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                output[i] = action(source[i]);
            }
            return output;
        }

        public static bool ForAll<T>(this IEnumerable<T> source, Func<T, bool> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            return source.Aggregate(action(source.First()), (accu, next) => accu & action(next));
        }
    }
    public static class StringExtensions
    {
        public static int Count(this string source, char c)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Count(s => s == c);
        }
        public static int Count(this string source, string c)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            System.Text.RegularExpressions.Regex regex = new(c);
            return regex.Matches(source).Count;
        }
    }
}
