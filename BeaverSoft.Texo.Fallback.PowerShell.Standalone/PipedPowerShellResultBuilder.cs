using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BeaverSoft.Texo.Core.Text;
using BeaverSoft.Texo.Core.View;
using BeaverSoft.Texo.Fallback.PowerShell.Core.Messages;
using BeaverSoft.Texo.Fallback.PowerShell.Standalone.NamedPipes;
using BeaverSoft.Texo.Fallback.PowerShell.Transforming;

namespace BeaverSoft.Texo.Fallback.PowerShell.Standalone
{
    public class PipedPowerShellResultBuilder : IPowerShellResultBuilder
    {
        private readonly Regex urlExpression = new Regex(@"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,4}\b([-a-zA-Z0-9@:%_\+.~#?&\/=]*)", RegexOptions.Compiled);
        private readonly IMessageSender sender;
        private Guid commandKey;

        public PipedPowerShellResultBuilder(IMessageSender sender)
        {
            this.sender = sender;
        }

        public void SetCommandKey(Guid commandKey)
        {
            this.commandKey = commandKey;
        }

        public ValueTask StartAsync(InputModel inputModel)
        {
            return new ValueTask(
                sender.SendAsync(
                    new CommandMessage(commandKey, MessageType.CommandStarted)));
        }

        public async ValueTask<Item> FinishAsync()
        {
            await sender.SendAsync(new CommandMessage(commandKey, MessageType.CommandFinished));
            return Item.Empty;
        }

        public ValueTask WriteAsync(string text)
        {
            return new ValueTask(
                sender.SendAsync(
                    new CommandMessage(
                        commandKey,
                        MessageType.CommandOutput,
                        GenerateAutoUrls(text))));
        }

        public ValueTask WriteAsync(string text, ConsoleColor foreground, ConsoleColor? background = null)
        {
            AnsiStringBuilder builder = PrebuildColoredText(text, foreground, background);

            return new ValueTask(
                sender.SendAsync(
                    new CommandMessage(
                        commandKey,
                        MessageType.CommandOutput,
                        builder.ToString())));
        }

        public ValueTask WriteLineAsync(string text)
        {
            return new ValueTask(
                sender.SendAsync(
                    new CommandMessage(
                        commandKey,
                        MessageType.CommandOutput,
                        GenerateAutoUrls(text) + Environment.NewLine)));
        }

        public ValueTask WriteLineAsync(string text, ConsoleColor foreground, ConsoleColor? background = null)
        {
            AnsiStringBuilder builder = PrebuildColoredText(text, foreground, background);
            builder.AppendLine();

            return new ValueTask(
                sender.SendAsync(
                    new CommandMessage(
                        commandKey,
                        MessageType.CommandOutput,
                        builder.ToString())));
        }

        public ValueTask WriteLineAsync()
        {
            return new ValueTask(
                sender.SendAsync(
                    new CommandMessage(
                        commandKey,
                        MessageType.CommandOutput,
                        Environment.NewLine)));
        }

        private AnsiStringBuilder PrebuildColoredText(string text, ConsoleColor foreground, ConsoleColor? background)
        {
            AnsiStringBuilder builder = new AnsiStringBuilder();
            builder.AppendForegroundFormat(foreground);

            if (background.HasValue)
            {
                builder.AppendBackgroundFormat(background.Value);
            }

            builder.Append(GenerateAutoUrls(text));
            builder.AppendFormattingReset();
            return builder;
        }

        private string GenerateAutoUrls(string text)
        {
            return urlExpression.Replace(text, UrlMatchEvaluator);
        }

        private string UrlMatchEvaluator(Match match)
        {
            return $"\u001b[999m{match.Value}|{match.Value}\u001b[998m";
        }
    }
}
