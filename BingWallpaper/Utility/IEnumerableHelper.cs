using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BingWallpaper.Utility
{
    /// <summary>
    /// Queryable extensions
    /// </summary>
    public static class IEnumerableHelper
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        public static IQueryable<TSource> DistinctBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return source.GroupBy(p => keySelector).Select(p => p.FirstOrDefault());
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        private static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        public static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');

            Type type = typeof(T);

            ParameterExpression arg = Expression.Parameter(type, "x");

            Expression expr = arg;

            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                System.Reflection.PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);

            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }

        public static IEnumerable<t> Randomize<t>(this IEnumerable<t> target)
        {
            Random random = new Random();
            return target.OrderBy(x => random.Next());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(new Random());
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (rng == null)
            {
                throw new ArgumentNullException("rng");
            }

            return source.ShuffleIterator(rng);
        }

        private static IEnumerable<T> ShuffleIterator<T>(
            this IEnumerable<T> source, Random rng)
        {
            List<T> buffer = source.ToList();

            for (int i = 0; i < buffer.Count; i++)
            {
                int j = rng.Next(i, buffer.Count);
                yield return buffer[j];

                buffer[j] = buffer[i];
            }
        }

        public static T ThrowIf<T>(this T val, Func<T, bool> predicate, Exception ex)
        {
            if (predicate(val))
            {
                throw ex;
            }

            return val;
        }

        public static IEnumerable<T> ThrowIfAny<T>(this IEnumerable<T> values, Func<T, bool> predicate, Exception ex)
        {
            if (values.Any(predicate))
            {
                throw ex;
            }

            return values;
        }

        /// <summary>
        /// var numbers = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        /// old way using or's
        /// var old = numbers.Where(x => x == 2 || x == 3 || x == 5 || x == 7);
        /// new way using In
        /// var primes = numbers.Where(x => x.In(2, 3, 5, 7));
        /// </summary>
        public static bool In<T>(this T source, params T[] list)
        {
            return list.ToList().Contains(source);
        }

        public static T MinOrDefault<T>(this IEnumerable<T> source, T defaultValue)
        {
            if (source.Any<T>())
            {
                return source.Min<T>();
            }

            return defaultValue;
        }

        public static T MaxOrDefault<T>(this IEnumerable<T> source, T defaultValue)
        {
            if (source.Any<T>())
            {
                return source.Max<T>();
            }

            return defaultValue;
        }
    }
}