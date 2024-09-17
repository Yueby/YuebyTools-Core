using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yueby.Core
{
    /// <summary>
    /// 表示一个可绑定的属性，提供值变化的事件通知。
    /// </summary>
    public interface IBindProperty
    {
        /// <summary>
        /// 当属性值发生变化时触发的事件。
        /// </summary>
        event Action<object> ValueChanged;

        /// <summary>
        /// 获取属性的当前值。
        /// </summary>
        object Value { get; }
    }

    /// <summary>
    /// 表示一个泛型可绑定的属性，提供值变化的事件通知。
    /// </summary>
    /// <typeparam name="T">属性值的类型。</typeparam>
    public interface IBindProperty<T> : IBindProperty
    {
        /// <summary>
        /// 当属性值发生变化时触发的事件。
        /// </summary>
        new event Action<T> ValueChanged;

        /// <summary>
        /// 获取或设置属性的当前值。
        /// </summary>
        new T Value { get; set; }
    }

    /// <summary>
    /// 表示一个泛型可绑定的属性，提供值变化的事件通知。
    /// </summary>
    /// <typeparam name="T">属性值的类型。</typeparam>
    [Serializable]
    public class BindProperty<T> : IBindProperty<T>
    {
        /// <summary>
        /// 当属性值发生变化时触发的事件。
        /// </summary>
        public event Action<T> ValueChanged;

        event Action<object> IBindProperty.ValueChanged
        {
            add => _valueChanged += value;
            remove => _valueChanged -= value;
        }

        [SerializeField]
        protected T _value;

        /// <summary>
        /// 获取或设置属性的当前值。
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value))
                    return;

                _value = value;
                ValueChanged?.Invoke(value);
                _valueChanged?.Invoke(value);
            }
        }

        object IBindProperty.Value => _value;

        private Action<object> _valueChanged;

        /// <summary>
        /// 初始化 <see cref="BindProperty{T}"/> 类的新实例。
        /// </summary>
        /// <param name="value">属性的初始值。</param>
        public BindProperty(T value) => _value = value;

        /// <summary>
        /// 将值显式转换为 <see cref="BindProperty{T}"/>。
        /// </summary>
        /// <param name="value">要转换的值。</param>
        public static explicit operator BindProperty<T>(T value) => new BindProperty<T>(value);

        /// <summary>
        /// 将 <see cref="BindProperty{T}"/> 隐式转换为值。
        /// </summary>
        /// <param name="binding">要转换的绑定属性。</param>
        public static implicit operator T(BindProperty<T> binding) => binding._value;
    }
}
