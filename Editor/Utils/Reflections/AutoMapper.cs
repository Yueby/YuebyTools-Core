using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace Yueby.Utils.Reflections
{
    public class AutoMapper
    {
        private static readonly ConcurrentDictionary<(Type Source, Type Target), Action<object, object>> MappingCache = new();

        // 泛型映射
        public static TTarget Map<TSource, TTarget>(TSource source) where TTarget : new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var target = new TTarget();
            Map(source, target);
            return target;
        }

        // 非泛型映射或源对象为 object 的映射
        public static void Map(object source, object target)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var sourceType = source.GetType();
            var targetType = target.GetType();

            var mappingAction = MappingCache.GetOrAdd((sourceType, targetType), _ => CreateMappingAction(sourceType, targetType));
            mappingAction(source, target);
        }

        // 源对象为 object，目标对象为泛型
        public static TTarget Map<TTarget>(object source) where TTarget : new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var target = new TTarget();
            Map(source, target);
            return target;
        }

        private static Action<object, object> CreateMappingAction(Type sourceType, Type targetType)
        {
            var sourceProperties = AccessTools.GetDeclaredProperties(sourceType);
            var targetProperties = AccessTools.GetDeclaredProperties(targetType);

            var sourceFields = AccessTools.GetDeclaredFields(sourceType);
            var targetFields = AccessTools.GetDeclaredFields(targetType);

            return (sourceObj, targetObj) =>
            {
                // 映射属性
                foreach (var targetProperty in targetProperties)
                {
                    var sourceProperty = sourceProperties.FirstOrDefault(p => p.Name == targetProperty.Name);
                    if (sourceProperty != null && sourceProperty.CanRead && targetProperty.CanWrite)
                    {
                        var value = sourceProperty.GetValue(sourceObj);
                        if (value != null && targetProperty.PropertyType != sourceProperty.PropertyType)
                        {
                            value = Convert.ChangeType(value, targetProperty.PropertyType);
                        }
                        targetProperty.SetValue(targetObj, value);
                    }
                }

                // 映射字段
                foreach (var targetField in targetFields)
                {
                    var sourceField = sourceFields.FirstOrDefault(f => f.Name == targetField.Name);
                    if (sourceField != null)
                    {
                        var value = sourceField.GetValue(sourceObj);
                        targetField.SetValue(targetObj, value);
                    }
                }
            };
        }
    }
}