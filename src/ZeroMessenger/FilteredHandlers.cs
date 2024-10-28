
namespace ZeroMessenger;

internal sealed class FilteredMessageHandler<T>(MessageHandler<T> handler, MessageFilter<T>[] filters) : AsyncMessageHandler<T>
{
    protected override ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        return new FilterIterator(handler, filters).InvokeRecursiveAsync(message, cancellationToken);
    }

    struct FilterIterator(MessageHandler<T> handler, MessageFilter<T>[] filters)
    {
        int index;

        public ValueTask InvokeRecursiveAsync(T message, CancellationToken cancellationToken)
        {
            if (MoveNextFilter(out var filter))
            {
                return filter.InvokeAsync(message, cancellationToken, InvokeRecursiveAsync);
            }

            handler.Handle(message);
            return default;
        }

        bool MoveNextFilter(out MessageFilter<T> next)
        {
            while (index < filters.Length)
            {
                next = filters[index];
                index++;
                return true;
            }

            next = default!;
            return false;
        }
    }
}

internal sealed class FilteredAsyncMessageHandler<T>(AsyncMessageHandler<T> handler, MessageFilter<T>[] filters) : AsyncMessageHandler<T>
{
    protected override ValueTask HandleAsyncCore(T message, CancellationToken cancellationToken = default)
    {
        return new FilterIterator(handler, filters).InvokeRecursiveAsync(message, cancellationToken);
    }

    struct FilterIterator(AsyncMessageHandler<T> handler, MessageFilter<T>[] filters)
    {
        int index;

        public ValueTask InvokeRecursiveAsync(T message, CancellationToken cancellationToken)
        {
            if (MoveNextFilter(out var filter))
            {
                return filter.InvokeAsync(message, cancellationToken, InvokeRecursiveAsync);
            }

            return handler.HandleAsync(message);
        }

        bool MoveNextFilter(out MessageFilter<T> next)
        {
            while (index < filters.Length)
            {
                next = filters[index];
                index++;
                return true;
            }

            next = default!;
            return false;
        }
    }
}