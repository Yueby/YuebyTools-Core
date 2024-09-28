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
            foreach (var targetProperty in targetType.GetProperties())
            {
                var sourceProperty = AccessTools.Property(sourceType, GetSourceName(targetProperty));
                if (sourceProperty != null && sourceProperty.CanRead && targetProperty.CanWrite)
                {
                    try
                    {
                        var value = sourceProperty.GetValue(sourceObj);
                        SetPropertyOrFieldValue(targetProperty, targetObj, value);
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"Error mapping property {targetProperty.Name}: {ex.Message}");
                    }
                }
            }
        }

        private static void MapFields(Type sourceType, object sourceObj, Type targetType, object targetObj)
        {
            foreach (var targetField in targetType.GetFields())
            {
                var sourceField = AccessTools.Field(sourceType, GetSourceName(targetField));
                if (sourceField != null)
                {
                    try
                    {
                        var value = sourceField.GetValue(sourceObj);
                        SetPropertyOrFieldValue(targetField, targetObj, value);
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"Error mapping field {targetField.Name}: {ex.Message}");
                    }
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

        private static void SetPropertyOrFieldValue(MemberInfo memberInfo, object targetObj, object value)
        {
            if (value == null)
            {
                if (memberInfo is PropertyInfo propertyInfo && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(targetObj, null);
                }
                else if (memberInfo is FieldInfo fieldInfo)
                {
                    fieldInfo.SetValue(targetObj, null);
                }

                return;
            }

            try
            {
                if (memberInfo is PropertyInfo property && property.CanWrite)
                {
                    if (property.PropertyType.IsInstanceOfType(value))
                    {
                        property.SetValue(targetObj, value);
                    }
                    else if (value is IConvertible)
                    {
                        property.SetValue(targetObj, Convert.ChangeType(value, property.PropertyType, System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
                else if (memberInfo is FieldInfo field)
                {
                    if (field.FieldType.IsInstanceOfType(value))
                    {
                        field.SetValue(targetObj, value);
                    }
                    else if (value is IConvertible)
                    {
                        field.SetValue(targetObj, Convert.ChangeType(value, field.FieldType, System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
            }
            catch (InvalidCastException ex)
            {
                Log.Info($"Error setting value for {memberInfo.Name}: {ex.Message}");
            }
        }
    }
}