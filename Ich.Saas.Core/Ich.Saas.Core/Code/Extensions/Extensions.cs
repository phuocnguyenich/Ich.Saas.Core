using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;

namespace Ich.Saas.Core.Code.Extensions
{
    public static class Extensions
    {
        public static int? GetId(this object obj, int? defaultId = null)
        {
            if (obj == null) return defaultId;
            if (int.TryParse(obj.ToString(), out int value))
                return value as int?;

            return defaultId;
        }

        public static int GetInt(this object obj, int defaultInt = 0)
        {
            if (obj == null) return defaultInt;

            if (int.TryParse(obj.ToString(), out int value))
                return value;

            return defaultInt;
        }

        public static string ToDate(this DateTime dt)
        {
            return string.Format("{0:d}", dt);
        }
        
        public static IMappingExpression<TSource, TDestination> Map<TSource, TDestination>(
            this IMappingExpression<TSource, TDestination> map,
            Expression<Func<TDestination, object>> selector,
            Action<IMemberConfigurationExpression<TSource, TDestination, object>> memberOptions)
        {
            map.ForMember(selector, memberOptions);
            return map;
        }
    }
}
