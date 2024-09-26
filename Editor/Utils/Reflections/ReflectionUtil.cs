using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Yueby.Utils.Reflections
{
    public static class ReflectionUtil
    {
        public static T MapToInterface<T>(object obj) where T : class
        {
            // 获取接口类型和对象类型
            Type interfaceType = typeof(T);
            Type objectType = obj.GetType();

            // 创建一个实现接口的动态类型
            object mappedObject = Activator.CreateInstance(interfaceType);

            // 遍历接口的所有属性
            foreach (PropertyInfo property in interfaceType.GetProperties())
            {
                // 获取对象中对应属性
                PropertyInfo objectProperty = objectType.GetProperty(property.Name);
                if (objectProperty != null && objectProperty.CanRead)
                {
                    // 将对象属性值赋给接口的属性
                    object value = objectProperty.GetValue(obj);
                    property.SetValue(mappedObject, value);
                }
            }
            return mappedObject as T;
        }
    }
}