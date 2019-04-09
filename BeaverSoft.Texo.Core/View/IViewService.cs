using System;
using System.Collections.Immutable;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Runtime;
using StrongBeaver.Core;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.View
{
    public interface IViewService :
        IInitialisable<IExecutor>,
        IDisposable,
        IMessageBusService<ISettingUpdatedMessage>,
        IMessageBusService<IVariableUpdatedMessage>
    {
        void Render(Input.Input input);

        void Render(IImmutableList<IItem> items);

        void RenderIntellisence(IImmutableList<IItem> items);

        void RenderProgress(IProgress progress);

        void Update(string key, IItem item);

        void Start();
    }
}
