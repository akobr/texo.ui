using BeaverSoft.Texo.Core.Streaming;
using BeaverSoft.Texo.Core.Transforming;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Transforming;
using StrongBeaver.Core.Services.Logging;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BeaverSoft.Texo.Fallback.PowerShell
{
    class PowerShellResultStreamBuilder : IPowerShellResultBuilder
    {
        private static Regex progressRegex = new Regex(@"\s\d{1,3}%\s", RegexOptions.Compiled);

        private readonly ILogService logger;
        private readonly IPipeline<OutputModel> pipelineOutput;
        private readonly IPipeline<OutputModel> pipelineError;

        private ReportableStream stream;
        private FormattableStreamWriter writer;
        private InputModel input;
        private bool containError;

        public PowerShellResultStreamBuilder(ILogService logger)
        {
            this.logger = logger;
            pipelineOutput = new Pipeline<OutputModel>(logger);
            pipelineError = new Pipeline<OutputModel>(logger);

            InitialisePipeline();
        }

        private void InitialisePipeline()
        {
            pipelineOutput.AddPipe(new OutputAutoUrl());
            pipelineOutput.AddPipe(new GitOutput());
            pipelineOutput.AddPipe(new GitStatusOutput());
            pipelineOutput.AddPipe(new GetChildItemOutput());

            pipelineError.AddPipe(new GitError());
        }

        public bool ContainError => containError;

        public ReportableStream Stream => stream;

        public bool Start(InputModel inputModel)
        {
            input = inputModel;
            containError = false;
            
            stream = new ReportableStream();
            writer = new FormattableStreamWriter(stream.Stream, Encoding.UTF8, 1024, true);
            return true;
        }

        public IStreamedItem BuildStreamItem()
        {
            return new StreamedItem(stream);
        }

        public Item Finish()
        {
            writer.Dispose();
            writer = null;
            return Item.Empty;
        }

        public void Write(string text)
        {
            writer.Write(text);
            writer.Flush();
        }

        public void Write(string text, ConsoleColor foreground, ConsoleColor background)
        {
            if (foreground != ConsoleColor.White)
            {
                writer.SetForegroundTextColor(foreground);
            }

            if (background != ConsoleColor.Black)
            {
                writer.SetBackgroundTextColor(background);
            }

            writer.Write(text);
            writer.ResetFormatting();
            writer.Flush();
        }

        public void WriteDebugLine(string text)
        {
            WriteColoredLine(text, ConsoleColor.Magenta);
        }

        public async void WriteErrorLine(string text)
        {
            containError = true;
            OutputModel output = await pipelineError.ProcessAsync(new OutputModel(text, input));

            // WriteColored(text, 255, 160, 122);
            writer.SetForegroundTextColor(ConsoleColor.Red);
            writer.Write(output.Output);
            writer.ResetFormatting();
            
            if (!output.NoNewLine)
            {
                writer.WriteLine();
            }

            writer.Flush();
        }

        public async void WriteLine(string text)
        {
            OutputModel output = await pipelineOutput.ProcessAsync(new OutputModel(text, input));

            writer.Write(output.Output);

            if (!output.NoNewLine)
            {
                writer.WriteLine();
            }

            writer.Flush();
        }

        public void WriteLine()
        {
            writer.WriteLine();
        }

        public void WriteVerboseLine(string text)
        {
            WriteColoredLine(text, ConsoleColor.Green);
        }

        public void WriteWarningLine(string text)
        {
            WriteColoredLine(text, ConsoleColor.Yellow);
        }

        private void WriteColored(string text, ConsoleColor color)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            writer.SetForegroundTextColor(color);
            writer.Write(text);
            writer.ResetFormatting();
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

        private void WriteColoredLine(string text, ConsoleColor color)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                writer.WriteLine();
                writer.Flush();
                return;
            }

            writer.SetForegroundTextColor(color);
            writer.Write(text);
            writer.ResetFormatting();
            writer.WriteLine();
            writer.Flush();
        }
    }
}
