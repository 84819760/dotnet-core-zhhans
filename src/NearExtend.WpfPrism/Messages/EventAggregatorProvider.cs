using Prism.Events;
using Prism.Ioc;

namespace NearExtend.WpfPrism.Messages
{
    internal static class EventAggregatorProvider
    {
        public static IEventAggregator GetEventAggregator() => ContainerLocator
            .Container?.Resolve<IEventAggregator>();
    }
}
