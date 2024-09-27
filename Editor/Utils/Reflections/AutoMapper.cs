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

        // ����ӳ��
        public static TTarget Map<TSource, TTarget>(TSource source) where TTarget : new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var target = new TTarget();
            Map(source, target);
            return target;
        }

        // �Ƿ���ӳ���Դ����Ϊ object ��ӳ��
        public static void Map(object source, object target)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var sourceType = source.GetType();
            var targetType = target.GetType();

            var mappingAction = MappingCache.GetOrAdd((sourceType, targetType), _ => CreateMappingAction(sourceType, targetType));
            mappingAction(source, target);
        }

        // Դ����Ϊ object��Ŀ�����Ϊ����
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
                // ӳ������
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

                // ӳ���ֶ�
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