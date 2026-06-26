using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustomCollection<T> : IEnumerable<T>
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);
    public void Remove(T item) => _items.Remove(item);

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public IEnumerable<T> GetReverseEnumerator()
    {
        throw new NotImplementedException();
    }
    
    public static IEnumerable<int> GenerateSequence(int start, int count)
    {
        throw new NotImplementedException();
    }
    
    public IEnumerable<T> FilterAndSort(Func<T, bool> predicate, Func<T, IComparable> keySelector)
    {
        throw new NotImplementedException();
    }
}
