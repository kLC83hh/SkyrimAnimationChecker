using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyrimAnimationChecker
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

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
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            U[] output = new U[source.Length];
            for (int i = 0; i < source.Count(); i++)
            {
                output[i] = action(source[i]);
            }
            return output;
        }

        public static bool ForAll<T>(this IEnumerable<T> source, Func<T, bool> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            return source.Aggregate(action(source.First()), (accu, next) => accu & action(next));
        }
    }
    public static class StringExtensions
    {
        public static int Count(this string source, char c)
        {
            if (source == null) throw new ArgumentNullException("source");

            return source.Count(s => s == c);
        }
        public static int Count(this string source, string c)
        {
            if (source == null) throw new ArgumentNullException("source");

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(c);
            return regex.Matches(source).Count();
        }
    }
}
