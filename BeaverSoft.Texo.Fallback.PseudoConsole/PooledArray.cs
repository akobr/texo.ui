using System;
using System.Buffers;

namespace BeaverSoft.Texo.Fallback.PseudoConsole
{
    internal struct PooledArray<T> : IDisposable
    {
        private readonly ArrayPool<T> pool;
        private readonly bool clearArray;

        public readonly T[] Array;

        public PooledArray(int minimumLength)
            : this (minimumLength, ArrayPool<T>.Shared) { }

        public PooledArray(int minimumLength, ArrayPool<T> pool, bool clearArray = false)
        {
            this.pool = pool ?? throw new ArgumentNullException(nameof(pool));
            this.clearArray = clearArray;

            Array = pool.Rent(minimumLength);
        }

        public void Dispose()
        {
            pool.Return(Array, clearArray);
        }
    }
}
