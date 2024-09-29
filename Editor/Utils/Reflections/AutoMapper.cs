using System;
using System.Collections.Concurrent;
using System.Reflection;
using HarmonyLib;
using YuebyTools.Core.Utils;

namespace Yueby.Utils.Reflections
{
    public class AutoMapper
    {
        private static readonly ConcurrentDictionary<(Type Source, Type Target), Action<object, object>> MappingCache = new();

        public static TTarget Map<TSource, TTarget>(TSource source) where TSource : class where TTarget : new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var target = new TTarget();
            Map(source, target);
            return target;
        }

        public static void Map(object source, object target)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var sourceType = source.GetType();
            var targetType = target.GetType();

            var mappingAction = MappingCache.GetOrAdd((sourceType, targetType), _ => CreateMappingAction(sourceType, targetType));
            mappingAction(source, target);
        }

        public static TTarget Map<TTarget>(object source) where TTarget : new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            var target = new TTarget();
            Map(source, target);
            return target;
        }

        private static Action<object, object> CreateMappingAction(Type sourceType, Type targetType)
        {
            return (sourceObj, targetObj) =>
            {
                MapProperties(sourceType, sourceObj, targetType, targetObj);
                MapFields(sourceType, sourceObj, targetType, targetObj);
            };
        }

        private static void MapProperties(Type sourceType, object sourceObj, Type targetType, object targetObj)
        {
            foreach (var targetProperty in AccessTools.GetDeclaredProperties(targetType))
            {
                var sourceProperty = AccessTools.Property(sourceType, GetSourceName(targetProperty));
                if (sourceProperty != null && sourceProperty.CanRead && targetProperty.CanWrite)
                {
                    try
                    {
                        var value = sourceProperty.GetValue(sourceObj);
                        Log.Info($"Mapping property {sourceProperty.Name} to {targetProperty.Name}, value: {value}");
                        targetProperty.SetValue(targetObj, value);
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"Error mapping property {targetProperty.Name}: {ex.Message}");
                    }
                }
                else
                {
                    Log.Info($"Property {targetProperty.Name} not found in source or not accessible.");
                }
            }
        }

        private static void MapFields(Type sourceType, object sourceObj, Type targetType, object targetObj)
        {
            foreach (var targetField in AccessTools.GetDeclaredFields(targetType))
            {
                var sourceField = AccessTools.Field(sourceType, GetSourceName(targetField));
                if (sourceField != null)
                {
                    try
                    {
                        var value = sourceField.GetValue(sourceObj);
                        Log.Info($"Mapping field {sourceField.Name} to {targetField.Name}, value: {value}");
                        targetField.SetValue(targetObj, value);
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"Error mapping field {targetField.Name}: {ex.Message}");
                    }
                }
                else
                {
                    Log.Info($"Field {targetField.Name} not found in source or not accessible.");
                }
            }
        }

        private static string GetSourceName(MemberInfo memberInfo)
        {
            if (Attribute.IsDefined(memberInfo, typeof(CustomMappingAttribute)))
            {
                var customMappingAttr = memberInfo.GetCustomAttribute<CustomMappingAttribute>();
                return customMappingAttr.SourceName; // 返回自定义名称
            }

            return memberInfo.Name; // 默认使用成员名称
        }

        private static bool IsMappingClass(Type type)
        {
            return Attribute.IsDefined(type, typeof(MappingClassAttribute));
        }
    }
}