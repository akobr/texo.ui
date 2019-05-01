using System.Collections.Generic;

namespace BeaverSoft.Texo.Core.Inputting.InputTree
{
    public abstract class ParameteriseNode : BaseNode
    {
        protected ParameteriseNode()
        {
            Parameters = new List<ParameterNode>();
        }

        public List<ParameterNode> Parameters { get; }

        public override string ToString()
        {
            return "root";
        }
    }
}
