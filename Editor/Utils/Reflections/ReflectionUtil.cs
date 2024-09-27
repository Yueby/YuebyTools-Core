using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using HarmonyLib;

namespace Yueby.Utils.Reflections
{
    public static class ReflectionUtil
    {
        public static T Create<T>(object source) where T : new()
        {
            // 创建目标对象
            var target = new T();

            // 获取源对象的类型
            var sourceType = source.GetType();

            // 获取目标对象的类型
            var targetType = typeof(T);

            // 映射属性
            foreach (var targetProperty in targetType.GetProperties())
            {
                // 使用 AccessTools 获取源对象的属性
                var sourceProperty = AccessTools.Property(sourceType, targetProperty.Name);
                if (sourceProperty != null && sourceProperty.CanRead && targetProperty.CanWrite)
                {
                    var value = sourceProperty.GetValue(source);
                    targetProperty.SetValue(target, value);
                }
            }

            // 映射字段
            foreach (var targetField in targetType.GetFields())
            {
                // 使用 AccessTools 获取源对象的字段
                var sourceField = AccessTools.Field(sourceType, targetField.Name);
                if (sourceField != null)
                {
                    var value = sourceField.GetValue(source);
                    targetField.SetValue(target, value);
                }
            }

            return target;
        }
    }
}