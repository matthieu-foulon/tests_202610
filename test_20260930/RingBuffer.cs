using System;
using System.Collections.Generic;
using System.Text;

namespace hsbc_test_20260930;

public class RingBuffer<T>
{
    private readonly T[] _data;
    private int _size;
    private int _readIndex;
    private int _writeIndex;

    public RingBuffer(int capacity)
    {
        _data = new T[capacity];
    }

    public int Capacity => _data.Length;
    public int Length => _size;
    public bool Empty => Length != 0;

    public bool Put(T value)
    {
        // early return, no more available space, caller decides what to do
        if(_size == _data.Length)
        {
            return false;
        }

        // store value and increment write index
        _data[_writeIndex++] = value;

        // wrap index
        if (_writeIndex == _data.Length)
        {
            _writeIndex = 0;
        }

        // for size direct access
        ++_size;

        return true;
    }

    public T? Get()
    {
        // early return, nothing available
        if(_size == 0)
        {
            return default;
        }

        // retrieve element, remove its reference from the container and increment read index
        T result = _data[_readIndex];
        _data[_readIndex++] = default!;

        // wrap index
        if (_readIndex == _data.Length)
        {
            _readIndex = 0;
        }

        // for size direct access
        --_size;

        return result;
    }

    public T? Peek()
    {
        // early return, nothing available
        if ( _size == 0 )
        {
            return default;
        }

        // retrieve element, no other action
        return _data[_readIndex];
    }
}
