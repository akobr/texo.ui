using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Windows;
using System.Windows.Documents;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Runtime;
using BeaverSoft.Texo.Core.View;
using StrongBeaver.Core.Messaging;

namespace BeaverSoft.Texo.View.WPF
{
    public class WpfViewService : IViewService
    {
        private readonly IWpfRenderService renderer;

        private IExecutor executor;
        private TexoControl control;

        public WpfViewService(IWpfRenderService renderer)
        {
            this.renderer = renderer;
        }

        public void Render(Input input)
        {
            // TODO
        }

        public void Render(IImmutableList<IItem> items)
        {
            List<Section> sections = new List<Section>(items.Count);

            foreach (IItem item in items)
            {
                sections.Add(renderer.Render(item));
            }

            if (sections.Count < 1)
            {
                return;
            }

            Section lastSection = sections[sections.Count - 1];
            lastSection.Loaded += HandleLastSectionLoaded;
            control.OutputDocument.Document.Blocks.AddRange(sections);
        }

        private void HandleLastSectionLoaded(object sender, RoutedEventArgs e)
        {
            Section section = (Section)sender;
            section.Loaded -= HandleLastSectionLoaded;
            section.BringIntoView();
        }

        public void RenderIntellisence(IImmutableList<IItem> items)
        {
            throw new NotImplementedException();
        }

        public void RenderProgress(IProgress progress)
        {
            throw new NotImplementedException();
        }

        public void Update(string key, IItem item)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            // TODO
        }

        public void Dispose()
        {
            // TODO
        }

        public void Initialise(IExecutor context)
        {
            executor = context;
        }

        public void Initialise(TexoControl context)
        {
            control = context;
        }

        void IMessageBusRecipient<ISettingUpdatedMessage>.ProcessMessage(ISettingUpdatedMessage message)
        {
            // TODO
        }

        void IMessageBusRecipient<IVariableUpdatedMessage>.ProcessMessage(IVariableUpdatedMessage message)
        {
            // TODO
        }
    }
}
