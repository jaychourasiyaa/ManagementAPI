using ManagementAPI.Contract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManagementAPI.Provider.Services
{
    public class SortingService : ISortingService
    {
        public IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy, bool isAscending)
        {
            if (string.IsNullOrEmpty(sortBy) || query == null)
            {
                return query;
            }
            
            //var property = typeof(T).GetProperty(sortBy, (BindingFlags)StringComparison.OrdinalIgnoreCase);
            var property = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                    .FirstOrDefault(p => p.Name.Equals(sortBy, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                throw new ArgumentException($"Property '{sortBy}' does not exist on type '{typeof(T).Name}'");
            }

            var parameter = Expression.Parameter(typeof(T), "e");
            var propertyAccess = Expression.Convert(Expression.Property(parameter, property), typeof(object));
            var keySelector = Expression.Lambda<Func<T, object>>(propertyAccess, parameter);

            return isAscending
                ? query.OrderBy(keySelector)
                : query.OrderByDescending(keySelector);
        }
    }
}
