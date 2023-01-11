using System;
using System.Collections.Generic;

namespace Neo.Utilities;

public class UnknownVariablesDictionary<TKey, TValue> : IDisposable
{
    public List<TKey> Keys { get; private set; } = new();
    public List<TValue> Values { get; private set; } = new();

    public void Add(UnknownVariable<TKey, TValue> unknownVariable)
    {
        Keys.Add(unknownVariable.Key);
        Values.Add(unknownVariable.Value);
    }

    private void ReleaseUnmanagedResources()
    {
        Keys = null;
        Values = null;
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
}