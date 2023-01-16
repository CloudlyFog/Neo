using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Utilities;

public class UnknownVariablesDictionary<TKey, TValue> : IDisposable
{
    public List<TKey> Keys { get; private set; } = new();
    public List<TValue> Values { get; private set; } = new();
    public List<string> Lines { get; private set; } = new();
    public List<Indexer> Indexers { get; private set; } = new();
    public string Digits { get; set; }


    public void Add(UnknownVariable<TKey, TValue> unknownVariable)
    {
        Keys.Add(unknownVariable.Key);
        Values.Add(unknownVariable.Value);
        Lines.Add(unknownVariable.Line);
        Indexers.Add(unknownVariable.Indexer);
    }

    private void ReleaseUnmanagedResources()
    {
        Keys = null;
        Values = null;
        Lines = null;
        Indexers = null;
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~UnknownVariablesDictionary()
    {
        Dispose(false);
    }
}

public class UnknownVariable<TKey, TValue>
{
    public TKey Key { get; set; }
    public TValue Value { get; set; }
    public string Line { get; set; }
    public Indexer Indexer { get; set; } = new();
}

public class Indexer
{
    public List<int> Indices { get; } = new();
    public List<string> Values { get; } = new();

    public static Indexer GetIndexer(IEnumerable<Indexer> indexers, Func<Indexer, bool> predicate)
    {
        return indexers.FirstOrDefault(predicate.Invoke);
    }
}