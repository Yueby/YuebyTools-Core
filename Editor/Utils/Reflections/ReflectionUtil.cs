using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace Yueby.Utils.Reflections
{
    public static class ReflectionUtil
    {
        public static T Map<T>(object source) where T : new()
        {
            var target = new T();
            Map(source, target);
            return target;
        }

        private static void Map(object source, object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var sourceType = source.GetType();
            var targetType = target.GetType();

            // Map properties
            foreach (var targetProperty in targetType.GetProperties())
            {
                var targetPropertyName = GetSourceName(targetProperty);
                var sourceProperty = AccessTools.Property(sourceType, targetPropertyName);

                if (sourceProperty != null && sourceProperty.CanRead)
                {
                    var value = sourceProperty.GetValue(source);
                    if (value != null)
                    {
                        if (IsMappingClass(targetProperty.PropertyType))
                        {
                            var targetValue = Activator.CreateInstance(targetProperty.PropertyType);
                            Map(value, targetValue);
                            targetProperty.SetValue(target, targetValue);
                        }
                        else if (IsGenericList(targetProperty.PropertyType))
                        {
                            var targetList = (IList)Activator.CreateInstance(targetProperty.PropertyType);
                            var sourceList = (IList)value;
                            foreach (var item in sourceList)
                            {
                                var targetItem = Activator.CreateInstance(targetProperty.PropertyType.GetGenericArguments()[0]);
                                Map(item, targetItem);
                                targetList.Add(targetItem);
                            }
                            targetProperty.SetValue(target, targetList);
                        }
                        else if (IsGenericDictionary(targetProperty.PropertyType))
                        {
                            var targetDict = (IDictionary)Activator.CreateInstance(targetProperty.PropertyType);
                            var sourceDict = (IDictionary)value;
                            foreach (var key in sourceDict.Keys)
                            {
                                var targetKey = key; // Assuming keys are primitive types
                                var targetValue = Activator.CreateInstance(targetProperty.PropertyType.GetGenericArguments()[1]);
                                Map(sourceDict[key], targetValue);
                                targetDict.Add(targetKey, targetValue);
                            }
                            targetProperty.SetValue(target, targetDict);
                        }
                        else
                        {
                            targetProperty.SetValue(target, value);
                        }
                    }
                }
            }

            // Map fields
            foreach (var targetField in targetType.GetFields())
            {
                var targetFieldName = GetSourceName(targetField);
                var sourceField = AccessTools.Field(sourceType, targetFieldName);

                if (sourceField != null)
                {
                    var value = sourceField.GetValue(source);
                    if (value != null)
                    {
                        if (IsMappingClass(targetField.FieldType))
                        {
                            var targetValue = Activator.CreateInstance(targetField.FieldType);
                            Map(value, targetValue);
                            targetField.SetValue(target, targetValue);
                        }
                        else if (IsGenericList(targetField.FieldType))
                        {
                            var targetList = (IList)Activator.CreateInstance(targetField.FieldType);
                            var sourceList = (IList)value;
                            foreach (var item in sourceList)
                            {
                                var targetItem = Activator.CreateInstance(targetField.FieldType.GetGenericArguments()[0]);
                                Map(item, targetItem);
                                targetList.Add(targetItem);
                            }
                            targetField.SetValue(target, targetList);
                        }
                        else if (IsGenericDictionary(targetField.FieldType))
                        {
                            var targetDict = (IDictionary)Activator.CreateInstance(targetField.FieldType);
                            var sourceDict = (IDictionary)value;
                            foreach (var key in sourceDict.Keys)
                            {
                                var targetKey = key; // Assuming keys are primitive types
                                var targetValue = Activator.CreateInstance(targetField.FieldType.GetGenericArguments()[1]);
                                Map(sourceDict[key], targetValue);
                                targetDict.Add(targetKey, targetValue);
                            }
                            targetField.SetValue(target, targetDict);
                        }
                        else
                        {
                            targetField.SetValue(target, value);
                        }
                    }
                }
            }
        }

        private static bool IsGenericDictionary(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
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

        public static bool IsMappingClass(Type type)
        {
            return Attribute.IsDefined(type, typeof(MappingClassAttribute));
        }

        private static bool IsGenericList(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}