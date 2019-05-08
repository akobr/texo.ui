using StrongBeaver.Core.Services.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Streaming.Observable
{
    public class ObservableStream<TItem> : IWritableObservableStream<TItem>
    {
        private const int DEFAULT_CAPACITY = 32;

        private readonly object streamLock = new object();
        private readonly ILogService logger;

        private ImmutableList<ObservationContext> observers;
        private TItem[] buffer;
        private int capacity;
        private bool isOpen;
        private int freeIndex;

        public ObservableStream()
            : this(null)
        {
            // no operation
        }

        public ObservableStream(ILogService logger)
        {
            this.logger = logger;
            capacity = DEFAULT_CAPACITY;
            observers = ImmutableList<ObservationContext>.Empty;
        }

        public bool IsClosed => !isOpen;

        public void Open()
        {
            lock (streamLock)
            {
                isOpen = true;
                buffer = new TItem[capacity];
                freeIndex = 0;
            }
        }

        public void Close()
        {
            lock (streamLock)
            {
                isOpen = false;
            }
        }

        public void Dispose()
        {
            Close();
            observers.Clear();
            buffer = null;
            freeIndex = 0;
        }

        public void RegisterObserver(IStreamObserver observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            observers = observers.Add(new ObservationContext { Observer = observer });
        }

        public void UnregisterObserver(IStreamObserver observer)
        {
            if (observer == null)
            {
                return;
            }

            int index = observers.FindIndex(context => context.Observer == observer);
            observers = observers.RemoveAt(index);
        }

        public void Write(IEnumerable<TItem> items)
        {
            if (items == null)
            {
                return;
            }

            lock (streamLock)
            {
                if (!isOpen)
                {
                    throw new InvalidOperationException("The observable stream is closed.");
                }

                WriteContext context = new WriteContext()
                {
                    StartIndex = freeIndex,
                    Observers = observers
                };

                foreach (TItem item in items)
                {
                    context.Item = item;
                    WriteItem(context);
                }

                NotifyObservers(context);
            }
        }

        private void NotifyObservers(WriteContext context)
        {
            foreach (ObservationContext observation in context.Observers)
            {
                try
                {
                    //Range range = new Range(observation.Position, freeIndex, buffer);
                    //int updateCount = observation.Observer.Update(range);
                    //observation.Position += Math.Max(0, Math.Min(updateCount, freeIndex, range.Length));
                }
                catch (Exception exception)
                {
                    logger.Error($"Error in stream observer '{observation.Observer.GetType().Name}'.", exception);
                }
            }
        }

        public Task WriteAsync(IEnumerable<TItem> items)
        {
            if (items == null)
            {
                return Task.CompletedTask;
            }

            return Task.Run(() => Write(items));
        }

        private void WriteItem(WriteContext context)
        {
            if (freeIndex >= buffer.Length)
            {
                ShiftOrResizeBuffer(context);
            }

            buffer[freeIndex] = context.Item;
        }

        private void ShiftOrResizeBuffer(WriteContext context)
        {
            int minimumIndex = observers.Min(context => context.Position);

            if (minimumIndex > 0)
            {
                if (minimumIndex >= freeIndex)
                {
                    ClearBuffer(context);
                }
                else
                {
                    ShiftBuffer(context, minimumIndex);
                }
            }
            else
            {
                ResizeBuffer();
            }
        }

        private void ShiftBuffer(WriteContext context, int minimumIndex)
        {
            TItem[] newBuffer = new TItem[capacity];
            int length = freeIndex - minimumIndex;
            Array.Copy(buffer, minimumIndex, newBuffer, 0, length);
            buffer = newBuffer;
            context.StartIndex -= length;
            freeIndex -= length;

            foreach (ObservationContext observation in observers)
            {
                observation.Position -= length;
            }
        }

        private void ResizeBuffer()
        {
            capacity = capacity * 2;
            TItem[] newBuffer = new TItem[capacity];
            Array.Copy(buffer, newBuffer, buffer.Length);
            buffer = newBuffer;
        }

        private void ClearBuffer(WriteContext contex)
        {
            buffer = new TItem[capacity];
            contex.StartIndex = 0;
            freeIndex = 0;
        }

        private class WriteContext
        {
            public int StartIndex;
            public TItem Item;
            public IImmutableList<ObservationContext> Observers;
        }

        private class ObservationContext
        {
            public IStreamObserver Observer;
            public int Position;
        }
    }
}
