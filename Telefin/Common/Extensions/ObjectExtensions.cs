using System;
using System.Globalization;

namespace Telefin.Common.Extensions
{
    internal static class ObjectExtensions
    {
        public static T? GetPropertySafely<T>(this object? obj, string propertyName, IFormatProvider? formatProvider = null)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return default;
            }

            var prop = obj.GetType().GetProperty(propertyName);

            if (prop == null || !prop.CanRead)
            {
                return default;
            }

            var value = prop.GetValue(obj);

            if (value is T typedValue)
            {
                return typedValue;
            }

            // Try automatic conversion to the requested type as fallback
            // The user might intentionally want the conversion
            try
            {
                return (T?)Convert.ChangeType(value, typeof(T), formatProvider ?? CultureInfo.InvariantCulture);
            }
            catch
            {
                return default;
            }
        }
    }
}
