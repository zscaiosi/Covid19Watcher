using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Covid19Watcher.Application.Helpers
{
    public static class EnumHelper
    {
        /// <summary>
        /// Gets description attribute's value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}