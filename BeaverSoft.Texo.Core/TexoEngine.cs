using System;
using System.Collections.Generic;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core;

namespace BeaverSoft.Texo.Core
{
    public class TexoEngine : IInitialisable<TextumConfiguration>, IDisposable
    {
        private readonly IRuntimeCoordinatorService runtime;
        private readonly IViewService view;
        private readonly ISettingService setting;

        internal TexoEngine(
            IRuntimeCoordinatorService runtime,
            IViewService view,
            ISettingService setting)
        {
            this.runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));
            this.view = view;
        }

        public IRuntimeCoordinatorService Runtime => runtime;

        public IViewService View => view;

        public void Initialise(TextumConfiguration configuration)
        {
            runtime.Initialise();
            setting.Configure(configuration);
        }

        public void Initialise(params Query[] commands)
        {
            Initialise((IEnumerable<Query>)commands);
        }

        public void Initialise(IEnumerable<Query> commands)
        {
            Initialise(TextumConfiguration.CreateDefault().AddCommands(commands));
        }

        public void Recorfigure(TextumConfiguration configuration)
        {
            setting.Configure(configuration);
        }

        public void Start()
        {
            runtime.Start();
        }

        public void Process(string input)
        {
            runtime.Process(input);
        }

        public void Dispose()
        {
            runtime.Dispose();
        }
    }
}
