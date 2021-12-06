using System.Threading.Tasks.Dataflow;

namespace DotNetCore_zhHans.Db.Import;

internal abstract class TargetBlockBase<T> : ITargetBlock<T>, IAsyncDisposable
{
    public abstract ITargetBlock<T> TargetBlock { get; }

    public DataflowMessageStatus OfferMessage(DataflowMessageHeader messageHeader
        , T messageValue
        , ISourceBlock<T>? source
        , bool consumeToAccept) => TargetBlock
        .OfferMessage(messageHeader, messageValue, source, consumeToAccept);

    public virtual Task<bool> SendAsync(T value) => TargetBlock.SendAsync(value);

    public void Complete() => TargetBlock.Complete();

    public void Fault(Exception exception) => TargetBlock.Fault(exception);

    public Task Completion => TargetBlock.Completion;

    public abstract ValueTask DisposeAsync();

    public static async ValueTask SetComplete<TType>(ITargetBlock<TType> block)
    {
        block.Complete();
        await block.Completion;
    }
}
