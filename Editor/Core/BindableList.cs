using System;
using System.Collections;
using System.Collections.Generic;

namespace Yueby.Core
{
    /// <summary>
    /// 表示一个可绑定的列表，提供列表内容变化的事件通知。
    /// </summary>
    [Serializable]
    public class BindableList<T> : BindProperty<List<T>>, IEnumerable<T>
    {
        /// <summary>
        /// 当列表内容发生变化时触发的事件。
        /// </summary>
        public event Action<List<T>> OnListChanged;

        /// <summary>
        /// 当列表项发生变化时触发的事件。
        /// </summary>
        public event Action<ItemChangedEventArgs<T>> OnItemChanged;

        /// <summary>
        /// 获取列表的当前项数。
        /// </summary>
        public int Count => Value.Count;

        /// <summary>
        /// 获取或设置指定索引处的项。
        /// </summary>
        /// <param name="index">项的索引。</param>
        /// <returns>指定索引处的项。</returns>
        public T this[int index]
        {
            get => Value[index];
            set
            {
                if (!EqualityComparer<T>.Default.Equals(Value[index], value))
                {
                    var oldValue = Value[index];
                    Value[index] = value;
                    NotifyItemChanged(index, oldValue, value);
                    NotifyListChanged();
                }
            }
        }

        /// <summary>
        /// 初始化 <see cref="BindableList{T}"/> 类的新实例。
        /// </summary>
        public BindableList() : base(new List<T>())
        {
        }

        /// <summary>
        /// 使用指定的集合初始化 <see cref="BindableList{T}"/> 类的新实例。
        /// </summary>
        /// <param name="collection">用于初始化列表的集合。</param>
        public BindableList(IEnumerable<T> collection) : base(new List<T>(collection))
        {
        }

        /// <summary>
        /// 将指定项添加到列表中。
        /// </summary>
        public void Add(T item)
        {
            Value.Add(item);
            OnCollectionChanged();
        }

        /// <summary>
        /// 将指定集合中的所有项添加到列表中。
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            Value.AddRange(items);
            OnCollectionChanged();
        }

        /// <summary>
        /// 移除列表中的所有项。
        /// </summary>
        public void Clear()
        {
            if (Value.Count > 0)
            {
                Value.Clear();
                OnCollectionChanged();
            }
        }

        /// <summary>
        /// 确定列表是否包含指定项。
        /// </summary>
        public bool Contains(T item) => Value.Contains(item);

        /// <summary>
        /// 搜索指定项并返回其索引。
        /// </summary>
        public int IndexOf(T item) => Value.IndexOf(item);

        /// <summary>
        /// 将指定项插入到列表中的指定索引处。
        /// </summary>
        public void Insert(int index, T item)
        {
            Value.Insert(index, item);
            OnCollectionChanged();
        }

        /// <summary>
        /// 在列表中根据谓词找到第一个匹配的项。
        /// </summary>
        public T Find(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match), "Match predicate cannot be null.");
            return Value.Find(match);
        }

        /// <summary>
        /// 移除列表中指定索引处的项。
        /// </summary>
        public void RemoveAt(int index)
        {
            if (index >= 0 && index < Value.Count)
            {
                var oldItem = Value[index];
                Value.RemoveAt(index);
                NotifyItemChanged(index, oldItem, default);
                OnCollectionChanged();
            }
        }

        /// <summary>
        /// 移除列表中的指定项。
        /// </summary>
        public bool Remove(T item)
        {
            int index = Value.IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取指定范围内的子列表。
        /// </summary>
        public List<T> GetRange(int index, int count) => Value.GetRange(index, count);

        /// <summary>
        /// 在排序列表中搜索元素。
        /// </summary>
        public int BinarySearch(T item, IComparer<T> comparer = null) => Value.BinarySearch(item, comparer ?? Comparer<T>.Default);

        /// <summary>
        /// 使用默认比较器对列表进行排序。
        /// </summary>
        public void Sort()
        {
            Value.Sort();
            NotifyListChanged();
        }

        /// <summary>
        /// 使用指定的比较器对列表进行排序。
        /// </summary>
        public void Sort(IComparer<T> comparer)
        {
            Value.Sort(comparer);
            NotifyListChanged();
        }

        /// <summary>
        /// 使用指定的比较函数对列表进行排序。
        /// </summary>
        public void Sort(Comparison<T> comparison)
        {
            Value.Sort(comparison);
            NotifyListChanged();
        }

        /// <summary>
        /// 减少列表的容量以接近其实际元素数量。
        /// </summary>
        public void TrimExcess() => Value.TrimExcess();

        /// <summary>
        /// 使列表可枚举。
        /// </summary>
        public IEnumerator<T> GetEnumerator() => Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void OnCollectionChanged() => NotifyListChanged();
        public void NotifyListChanged() => OnListChanged?.Invoke(Value);

        private void NotifyItemChanged(int index, T oldValue, T newValue)
        {
            OnItemChanged?.Invoke(new ItemChangedEventArgs<T>(index, oldValue, newValue));
        }
    }

    /// <summary>
    /// 表示列表项变化的事件参数。
    /// </summary>
    public class ItemChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 获取发生变化的项的索引。
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// 获取变化前的项的值。
        /// </summary>
        public T OldValue { get; }

        /// <summary>
        /// 获取变化后的项的值。
        /// </summary>
        public T NewValue { get; }

        /// <summary>
        /// 初始化 <see cref="ItemChangedEventArgs{T}"/> 类的新实例。
        /// </summary>
        /// <param name="index">发生变化的项的索引。</param>
        /// <param name="oldValue">变化前的项的值。</param>
        /// <param name="newValue">变化后的项的值。</param>
        public ItemChangedEventArgs(int index, T oldValue, T newValue)
        {
            Index = index;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}