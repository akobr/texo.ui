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
        IMessageBusService<IVariableUpdatedMessage>,
        IMessageBusService<IClearViewOutputMessage>,
        IMessageBusService<PromptUpdateMessage>
    {
        void SetInput(string input);

        void AddInput(string append);

        void Render(Input.Input input, IImmutableList<IItem> items);

        void RenderIntellisence(Input.Input input, IImmutableList<IItem> items);

        void RenderProgress(IProgress progress);

        void Update(string key, IItem item);

        void Start();
    }
}
