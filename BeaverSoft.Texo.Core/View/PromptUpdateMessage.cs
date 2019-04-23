using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.View
{
    public class PromptUpdateMessage : ServiceMessage
    {
        public PromptUpdateMessage(string prompt)
        {
            Prompt = prompt;
        }

        public string Prompt { get; }
    }
}
