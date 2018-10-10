using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.InputTree
{
    public abstract class ParameteriseNode : BaseNode
    {
        protected ParameteriseNode()
        {
            Parameters = new List<ParameterNode>();
        }

        public List<ParameterNode> Parameters { get; }
    }
}
