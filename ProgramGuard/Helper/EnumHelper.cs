using System;
using System.ComponentModel;
using System.Reflection;

namespace ProgramGuard.Helper
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
