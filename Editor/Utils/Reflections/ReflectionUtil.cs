using System;
using System.Reflection;
using Yueby.Core.Utils;

namespace Yueby.Utils.Reflections
{
    public static class ReflectionUtil
    {
        public static readonly BindingFlags all =
            BindingFlags.GetField
            | BindingFlags.GetProperty
            | BindingFlags.Instance
            | BindingFlags.NonPublic
            | BindingFlags.Public
            | BindingFlags.SetField
            | BindingFlags.SetProperty
            | BindingFlags.Static;

        public static readonly BindingFlags allDeclared = all | BindingFlags.DeclaredOnly;

        public static object GetFieldValue(this Type type, string fieldName, object obj) =>
            Field(type, fieldName).GetValue(obj);

        public static FieldInfo Field(Type type, string name)
        {
            if ((object)type == null)
            {
                YuebyLogger.LogInfo($"ReflectionUtil.Field: type is null");
                return null;
            }

            if (name == null)
            {
                YuebyLogger.LogInfo($"ReflectionUtil.Field: name is null");
                return null;
            }

            FieldInfo fieldInfo = FindIncludingBaseTypes(type, (Type t) => t.GetField(name, all));
            if ((object)fieldInfo == null)
            {
                YuebyLogger.LogInfo(
                    $"ReflectionUtil.Field: Could not find field for type {type} and name {name}"
                );
            }

            return fieldInfo;
        }

        public static T FindIncludingBaseTypes<T>(Type type, Func<Type, T> func)
            where T : class
        {
            do
            {
                T val = func(type);
                if (val != null)
                {
                    return val;
                }

                type = type.BaseType;
            } while ((object)type != null);
            return null;
        }
    }
}
