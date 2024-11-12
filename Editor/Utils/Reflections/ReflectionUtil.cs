using System;
using HarmonyLib;

namespace Yueby.Utils.Reflections
{
    public static class ReflectionUtil
    {
        public static object GetFieldValue(this Type type, string fieldName, object obj)
        {
            return type.GetField(fieldName).GetValue(obj);
        }
    }
}
