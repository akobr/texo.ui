using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Actions;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core
{
    public class TexoEngine : IInitialisable<TexoConfiguration>, IDisposable
    {
        private readonly IRuntimeCoordinatorService runtime;
        private readonly IViewService view;
        private readonly IActionFactoryRegister actionRegister;
        private readonly ISettingService setting;
        private readonly ILogService logger;

        internal TexoEngine(
            IRuntimeCoordinatorService runtime,
            IViewService view,
            IActionFactoryRegister actionRegister,
            ISettingService setting,
            ILogService logger)
        {
            this.runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));
            this.actionRegister = actionRegister ?? throw new ArgumentNullException(nameof(actionRegister));
            this.view = view;
            this.logger = logger;
        }

        public static Version Version => new Version(0, 9, 8, 3);

        public IRuntimeCoordinatorService Runtime => runtime;

        public IViewService View => view;

        public ILogService Logger => logger;

        public void Initialise(TexoConfiguration configuration)
        {
            runtime.Initialise();
            setting.Configure(configuration);
        }

        public Task InitialiseAsync(TexoConfiguration configuration)
        {
            return Task.Run(() => Initialise(configuration));
        }

        public void Initialise(params Query[] commands)
        {
            Initialise((IEnumerable<Query>)commands);
        }

        public Task InitialiseAsync(params Query[] commands)
        {
            return Task.Run(() => Initialise(commands));
        }

        public void Initialise(IEnumerable<Query> commands)
        {
            Initialise(TexoConfiguration.CreateDefault().AddCommands(commands));
        }

        public Task InitialiseAsync(IEnumerable<Query> commands)
        {
            return Task.Run(() => Initialise(commands));
        }

        public void Recorfigure(TexoConfiguration configuration)
        {
            setting.Configure(configuration);
        }

        public void RegisterAction(IActionFactory factory, params string[] actionNames)
        {
            actionRegister.RegisterFactory(factory, actionNames);
        }

        public void Start()
        {
            runtime.Start();
        }

        public Task ProcessAsync(string input)
        {
            return runtime.ProcessAsync(input);
        }

        public void Dispose()
        {
            runtime.Dispose();
        }
    }
}
