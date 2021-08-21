using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Interop;

using NotifyCollectionChangedAction = Microsoft.UI.Xaml.Interop.NotifyCollectionChangedAction;

// Copied from https://github.com/microsoft/Xaml-Controls-Gallery/blob/winui3preview/XamlControlsGallery/CollectionsInterop.cs
namespace AppUIBasics
{
    // .NET collection types are tightly coupled with WUX types - e.g., ObservableCollection<T>
    // maps to WUX.INotifyCollectionChanged, and creates WUX.NotifyCollectionChangedEventArgs
    // when raising its INCC event.  This is a problem because we've switched everything else over
    // to use MUX types, such that creating WUX types raises an RPC_E_WRONG_THREAD error
    // due to DXamlCore not being initialized.  For the purposes of our tests, we're providing
    // our own implementation of ObservableCollection<T> that implements MUX.INotifyCollectionChanged.
    public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ReentrancyGuard _reentrancyGuard = null;

        private class ReentrancyGuard : IDisposable
        {
            private readonly ObservableCollection<T> _owningCollection;

            public ReentrancyGuard(ObservableCollection<T> owningCollection)
            {
                owningCollection.CheckReentrancy();
                owningCollection._reentrancyGuard = this;
                _owningCollection = owningCollection;
            }

            public void Dispose() => _owningCollection._reentrancyGuard = null;
        }

        public ObservableCollection() : base() { }

        public ObservableCollection(IList<T> list) : base(list.ToList()) { }

        public ObservableCollection(IEnumerable<T> collection) : base(collection.ToList()) { }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void Move(int oldIndex, int newIndex) => MoveItem(oldIndex, newIndex);

        protected IDisposable BlockReentrancy() => new ReentrancyGuard(this);

        protected void CheckReentrancy()
        {
            if (_reentrancyGuard != null)
            {
                throw new InvalidOperationException("Collection cannot be modified in a collection changed handler.");
            }
        }

        protected override void ClearItems()
        {
            CheckReentrancy();

            TestBindableVector<T> oldItems = new(this);

            base.ClearItems();
            OnCollectionChanged(
                NotifyCollectionChangedAction.Reset,
                null, oldItems, 0, 0);
        }

        protected override void InsertItem(int index, T item)
        {
            CheckReentrancy();

            TestBindableVector<T> newItem = new();
            newItem.Add(item);

            base.InsertItem(index, item);
            OnCollectionChanged(
                NotifyCollectionChangedAction.Add,
                newItem, null, index, 0);
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            CheckReentrancy();

            TestBindableVector<T> oldItem = new();
            oldItem.Add(this[oldIndex]);
            TestBindableVector<T> newItem = new(oldItem);

            T item = this[oldIndex];
            base.RemoveAt(oldIndex);
            base.InsertItem(newIndex, item);
            OnCollectionChanged(
                NotifyCollectionChangedAction.Move,
                newItem, oldItem, newIndex, oldIndex);
        }

        protected override void RemoveItem(int index)
        {
            CheckReentrancy();

            TestBindableVector<T> oldItem = new();
            oldItem.Add(this[index]);

            base.RemoveItem(index);
            OnCollectionChanged(
                NotifyCollectionChangedAction.Remove,
                null, oldItem, 0, index);
        }

        protected override void SetItem(int index, T item)
        {
            CheckReentrancy();

            TestBindableVector<T> oldItem = new();
            oldItem.Add(this[index]);
            TestBindableVector<T> newItem = new();
            newItem.Add(item);

            base.SetItem(index, item);
            OnCollectionChanged(
                NotifyCollectionChangedAction.Replace,
                newItem, oldItem, index, index);
        }

        protected virtual void OnCollectionChanged(
            NotifyCollectionChangedAction action,
            IBindableVector newItems,
            IBindableVector oldItems,
            int newIndex,
            int oldIndex) => OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItems, oldItems, newIndex, oldIndex));

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            using (BlockReentrancy())
            {
                CollectionChanged?.Invoke(this, e);
            }
        }

#pragma warning disable 0067 // PropertyChanged is never used, raising a warning, but it's needed to implement INotifyPropertyChanged.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067
    }

    public class TestBindableVector<T> : IList<T>, IBindableVector
    {
        private readonly IList<T> _implementation;

        public TestBindableVector() => _implementation = new List<T>();
        public TestBindableVector(IList<T> list) => _implementation = new List<T>(list);

        public T this[int index] { get => _implementation[index]; set => _implementation[index] = value; }

        public int Count => _implementation.Count;

        public virtual bool IsReadOnly => _implementation.IsReadOnly;

        public void Add(T item) => _implementation.Add(item);

        public void Clear() => _implementation.Clear();

        public bool Contains(T item) => _implementation.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _implementation.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _implementation.GetEnumerator();

        public int IndexOf(T item) => _implementation.IndexOf(item);

        public void Insert(int index, T item) => _implementation.Insert(index, item);

        public bool Remove(T item) => _implementation.Remove(item);

        public void RemoveAt(int index) => _implementation.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => _implementation.GetEnumerator();

        public object GetAt(uint index) => _implementation[(int)index];

        public IBindableVectorView GetView() => new TestBindableVectorView<T>(_implementation);

        public bool IndexOf(object value, out uint index)
        {
            int indexOf = _implementation.IndexOf((T)value);

            if (indexOf >= 0)
            {
                index = (uint)indexOf;
                return true;
            }
            else
            {
                index = 0;
                return false;
            }
        }

        public void SetAt(uint index, object value) => _implementation[(int)index] = (T)value;

        public void InsertAt(uint index, object value) => _implementation.Insert((int)index, (T)value);

        public void RemoveAt(uint index) => _implementation.RemoveAt((int)index);

        public void Append(object value) => _implementation.Add((T)value);

        public void RemoveAtEnd() => _implementation.RemoveAt(_implementation.Count - 1);

        public uint Size => (uint)_implementation.Count;

        public IBindableIterator First() => new TestBindableIterator<T>(_implementation);
    }

    public class TestBindableVectorView<T> : TestBindableVector<T>, IBindableVectorView
    {
        public TestBindableVectorView(IList<T> list) : base(list) { }

        public override bool IsReadOnly => true;
    }

    public class TestBindableIterator<T> : IBindableIterator
    {
        private readonly IEnumerator<T> _enumerator;

        public TestBindableIterator(IEnumerable<T> enumerable) => _enumerator = enumerable.GetEnumerator();

        public bool MoveNext() => _enumerator.MoveNext();

        public object Current => _enumerator.Current;

        public bool HasCurrent => _enumerator.Current != null;
    }
}