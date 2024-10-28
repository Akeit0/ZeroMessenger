namespace ZeroMessenger;

public static class MessageSubscriberFilterExtensions
{
    public static IMessageSubscriber<TMessage> WithFilter<TMessage, TFilter>(this IMessageSubscriber<TMessage> subscriber)
        where TFilter : MessageFilter<TMessage>, new()
    {
        return WithFilter(subscriber, new TFilter());
    }

    public static IMessageSubscriber<T> WithFilter<T>(this IMessageSubscriber<T> subscriber, Func<T, CancellationToken, Func<T, CancellationToken, ValueTask>, ValueTask> filter)
    {
        return WithFilter(subscriber, new AnonymousMessageFilter<T>(filter));
    }

    public static IMessageSubscriber<T> WithFilter<T>(this IMessageSubscriber<T> subscriber, Predicate<T> predicate)
    {
        return WithFilter(subscriber, new PredicateFilter<T>(predicate));
    }

    public static IMessageSubscriber<TMessage> WithFilter<TMessage, TFilter>(this IMessageSubscriber<TMessage> subscriber, TFilter filter)
        where TFilter : MessageFilter<TMessage>
    {
        if (subscriber is FilteredMessageSubscriber<TMessage> filteredSubscriber)
        {
            var array = new MessageFilter<TMessage>[filteredSubscriber.Filters.Length + 1];
            filteredSubscriber.Filters.AsSpan().CopyTo(array);
            array[^1] = filter;
            return new FilteredMessageSubscriber<TMessage>(filteredSubscriber.Subscriber, array);
        }

        return new FilteredMessageSubscriber<TMessage>(subscriber, [filter]);
    }

    public static IMessageSubscriber<T> WithFilters<T>(this IMessageSubscriber<T> subscriber, ReadOnlySpan<MessageFilter<T>> filters)
    {
        if (subscriber is FilteredMessageSubscriber<T> filteredSubscriber)
        {
            var array = new MessageFilter<T>[filteredSubscriber.Filters.Length + 1];
            filteredSubscriber.Filters.AsSpan().CopyTo(array);
            filters.CopyTo(array.AsSpan(filteredSubscriber.Filters.Length));
            return new FilteredMessageSubscriber<T>(filteredSubscriber.Subscriber, array);
        }

        return new FilteredMessageSubscriber<T>(subscriber, filters.ToArray());
    }

    public static IMessageSubscriber<T> WithFilters<T>(this IMessageSubscriber<T> subscriber, params MessageFilter<T>[] filters)
    {
        if (subscriber is FilteredMessageSubscriber<T> filteredSubscriber)
        {
            var array = new MessageFilter<T>[filteredSubscriber.Filters.Length + 1];
            filteredSubscriber.Filters.AsSpan().CopyTo(array);
            filters.CopyTo(array.AsSpan(filteredSubscriber.Filters.Length));
            return new FilteredMessageSubscriber<T>(filteredSubscriber.Subscriber, array);
        }

        return new FilteredMessageSubscriber<T>(subscriber, filters);
    }

}