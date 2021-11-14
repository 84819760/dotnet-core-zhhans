using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCorezhHans.Base.Interfaces;
using DotNetCorezhHans.Messages;

namespace DotNetCorezhHans.TranslTasks
{
    internal abstract class TaskBase
    {
        protected readonly TranslManager translManager;

        public TaskBase(TranslManager translManager) => this.translManager = translManager;

        public abstract Task<PageControlType> Run();

        public IProgress Progress => translManager.Progress;

        public bool IsCancel => translManager.IsCancel;
    }
}
