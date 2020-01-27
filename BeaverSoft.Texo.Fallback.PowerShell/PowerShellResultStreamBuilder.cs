using System;
using System.Text;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Streaming;
using BeaverSoft.Texo.Core.Transforming;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Transforming;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    public class PowerShellResultStreamBuilder : IPowerShellResultBuilder
    {
        private readonly ILogService logger;
        private readonly IPipeline<OutputModel> pipelineOutput;

        private ReportableStream stream;
        private FormattableStreamWriter writer;
        private InputModel input;

        public PowerShellResultStreamBuilder(ILogService logger)
        {
            this.logger = logger;
            pipelineOutput = new Pipeline<OutputModel>(logger);

            InitialisePipeline();
        }

        private void InitialisePipeline()
        {
            pipelineOutput.AddPipe(new OutputAutoUrl());
            pipelineOutput.AddPipe(new GitOutput(logger));
            pipelineOutput.AddPipe(new GetChildItemOutput());
        }

        public ReportableStream Stream => stream;

        public ValueTask StartAsync(InputModel inputModel)
        {
            input = inputModel;

            stream = new ReportableStream();
            writer = new FormattableStreamWriter(stream.Stream, Encoding.UTF8, 1024, true);
            return new ValueTask();
        }

        public IStreamedItem BuildStreamItem()
        {
            return new StreamedItem(stream);
        }

        public ValueTask<Item> FinishAsync()
        {
            writer?.Dispose();
            writer = null;
            return new ValueTask<Item>(Item.Empty);
        }

        public ValueTask WriteAsync(string text)
        {
            writer.Write(text);
            writer.Flush();
            return new ValueTask();
        }

        public ValueTask WriteAsync(string text, ConsoleColor foreground, ConsoleColor? background = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new ValueTask();
            }

            writer.SetForegroundTextColor(foreground);

            if (background.HasValue)
            {
                writer.SetBackgroundTextColor(background.Value);
            }

            writer.Write(text);
            writer.ResetFormatting();
            writer.Flush();
            return new ValueTask();
        }

        public async ValueTask WriteErrorLineAsync(string text)
        {
            OutputModel output = new OutputModel(text, input);
            output.Flags.Add(TransformationFlags.ERROR);
            output = await pipelineOutput.ProcessAsync(output);

            writer.SetForegroundTextColor(ConsoleColor.Red);
            writer.Write(output.Output);
            writer.ResetFormatting();

            if (!output.NoNewLine)
            {
                writer.WriteLine();
            }

            writer.Flush();
        }

        public async ValueTask WriteLineAsync(string text)
        {
            OutputModel output = await pipelineOutput.ProcessAsync(new OutputModel(text, input));

            writer.Write(output.Output);

            if (!output.NoNewLine)
            {
                writer.WriteLine();
            }

            writer.Flush();
        }

        public ValueTask WriteLineAsync(string text, ConsoleColor foreground, ConsoleColor? background = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                writer.WriteLine();
                writer.Flush();
                return new ValueTask();
            }

            writer.SetForegroundTextColor(foreground);

            if (background.HasValue)
            {
                writer.SetBackgroundTextColor(background.Value);
            }

            writer.Write(text);
            writer.ResetFormatting();
            writer.WriteLine();
            writer.Flush();
            return new ValueTask();
        }

        public ValueTask WriteLineAsync()
        {
            writer.WriteLine();
            return new ValueTask();
        }

        private void WriteColored(string text, byte red, byte green, byte blue)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            writer.SetForegroundTextColor(red, green, blue);
            writer.Write(text);
            writer.ResetFormatting();
        }
    }
}
