using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.InputTree
{
    public class ParameterNode : BaseNode
    {
        public ParameterNode(Parameter parameter)
        {
            Parameter = parameter;
        }

        public Parameter Parameter { get; }

        public override NodeTypeEnum Type => NodeTypeEnum.Parameter;
    }
}
