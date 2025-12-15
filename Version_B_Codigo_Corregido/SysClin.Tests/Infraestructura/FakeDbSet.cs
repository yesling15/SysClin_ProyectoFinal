using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

public class FakeDbSet<T> : DbSet<T>, IQueryable<T>, IEnumerable<T> where T : class
{
    private readonly ObservableCollection<T> _data;
    private readonly IQueryable _query;

    public FakeDbSet(List<T> data)
    {
        _data = new ObservableCollection<T>(data);
        _query = _data.AsQueryable();
    }

    public override T Add(T item)
    {
        _data.Add(item);
        return item;
    }

    public override T Remove(T item)
    {
        _data.Remove(item);
        return item;
    }

    public override IEnumerable<T> AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
            _data.Add(item);

        return items;
    }

    public override IEnumerable<T> RemoveRange(IEnumerable<T> items)
    {
        foreach (var item in items)
            _data.Remove(item);

        return items;
    }
        
    public Type ElementType => _query.ElementType;
    public Expression Expression => _query.Expression;
    public IQueryProvider Provider => _query.Provider;

    public override ObservableCollection<T> Local => _data;

    public IEnumerator<T> GetEnumerator()
    {
        return _data.GetEnumerator();
    }
}