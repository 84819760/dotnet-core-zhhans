using Prism.Events;

namespace NearExtend.WpfPrism.Messages
{
    public class MessageBase<TResult, TTaarget> : PubSubEvent<TTaarget>
        where TResult : EventBase, new()
    {
        protected TResult GetEvent() => EventAggregatorProvider
                .GetEventAggregator()?
                .GetEvent<TResult>();

        public static TResult Instance { get; } =
            new MessageBase<TResult, TTaarget>().GetEvent() ?? new TResult();   
    }
}
