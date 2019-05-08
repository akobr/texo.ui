using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Inputting.Tree;
using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Inputting
{
    public class InputTreeUpdatedMessage : ServiceMessage, IInputTreeUpdatedMessage
    {
        public InputTreeUpdatedMessage(
            TextumConfiguration configuration,
            InputTree inputTree)
        {
            Configuration = configuration;
            InputTree = inputTree;
        }

        public TextumConfiguration Configuration { get; }

        public InputTree InputTree { get; }
    }
}
