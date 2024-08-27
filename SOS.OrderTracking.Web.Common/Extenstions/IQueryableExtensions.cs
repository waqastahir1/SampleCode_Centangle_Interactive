using System;
using System.Linq;
using System.Linq.Expressions;

namespace SOS.OrderTracking.Web.Common.Data
{
    public static class IQueryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderBy(ToLambda<T>(propertyName));
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            return source.OrderByDescending(ToLambda<T>(propertyName));
        }

        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }

        public static bool IsAlive(this object obj)
        {
            if (obj == null)
                return false;

            try
            {
                obj.ToString();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsDisposed(this object obj)
        {
            try
            {
                obj.ToString();
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }
    }
}
