using BeaverSoft.Texo.Core.Configuration;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Input
{
    public class InputTreeUpdatedMessage : ServiceMessage, IInputTreeUpdatedMessage
    {
        public InputTreeUpdatedMessage(
            TextumConfiguration configuration,
            InputTree.InputTree inputTree)
        {
            Configuration = configuration;
            InputTree = inputTree;
        }

        public TextumConfiguration Configuration { get; }

        public InputTree.InputTree InputTree { get; }
    }
}
