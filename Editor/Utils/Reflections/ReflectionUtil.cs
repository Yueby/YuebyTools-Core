using System;
using HarmonyLib;

namespace Yueby.Utils.Reflections
{
    public static class ReflectionUtil
    {
        public static object GetFieldValue(this Type type, string fieldName, object obj) => type.Field(fieldName).GetValue(obj);
    }
}