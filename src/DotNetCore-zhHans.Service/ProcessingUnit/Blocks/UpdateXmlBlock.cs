using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using DotNetCorezhHans.Base.Interfaces;

namespace DotNetCoreZhHans.Service.ProcessingUnit
{
    /// <summary>
    /// 负责更新XML
    /// </summary>
    internal class UpdateXmlBlock : UnitBase<NodeCacheData>
    {
        private readonly ActionBlock<NodeCacheData> block;
        private int count;

        public UpdateXmlBlock(ITransmitData transmits) : base(transmits)
        {
            var execOption = CreateExecutionDataflowBlockOption(1);
            block = new(Handler, execOption);
        }

        protected override ITargetBlock<NodeCacheData> TargetBlock => block;

        private Task Handler(NodeCacheData obj)
        {
            var root = obj.RootNode;
            if (!root.IsUpdateXml &&  root.IsCompletion)
            {
                XmlHelper.ReplaceXml(root);
                SendMsg();
                root.Clear();
            }
            return Task.CompletedTask;
        }

        private void SendMsg()
        {
            Interlocked.Increment(ref count);
            Transmits.Progress.Master.Title = $"更新 : {count}";
        }

        public override Task Complete() => SetComplete(block);
    }
}
