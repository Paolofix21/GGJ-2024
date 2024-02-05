using System.Collections.Generic;
using System.Linq;

namespace Code.Utilities {
    public delegate void ListAddRemoveEventHandler<in T>(T element);

    public sealed class SmartCollection<T> {
        #region Public Variables
        public event ListAddRemoveEventHandler<T> OnAdded, OnRemoved;
        public event System.Action OnCleared;
        #endregion

        #region Private Variables
        private readonly List<T> _list = new();
        #endregion

        #region Public Methods
        public void Add(T element) {
            _list.Add(element);
            OnAdded?.Invoke(element);
        }

        public void AddRange(IEnumerable<T> elements) {
            var array = elements.ToArray();

            _list.AddRange(array);

            foreach (var element in array)
                OnRemoved?.Invoke(element);
        }

        public void Remove(T element) {
            if (!_list.Remove(element))
                return;

            OnRemoved?.Invoke(element);

            if (_list.Count <= 0)
                OnCleared?.Invoke();
        }

        public void RemoveAt(int index) {
            if (index < 0 || index >= _list.Count)
                return;

            var element = _list[index];
            _list.RemoveAt(index);
            OnRemoved?.Invoke(element);

            if (_list.Count <= 0)
                OnCleared?.Invoke();
        }

        public void ForeEach(System.Action<T> action) => _list.ForEach(action);
        #endregion
    }
}