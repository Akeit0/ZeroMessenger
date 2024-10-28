using System.Runtime.CompilerServices;

namespace ZeroMessenger;

public abstract class MessageHandler<T> : MessageHandlerNode<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Handle(T message)
    {
        HandleCore(message);
    }

    protected virtual void HandleCore(T message) { }
}