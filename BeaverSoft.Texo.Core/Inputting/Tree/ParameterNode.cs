using BeaverSoft.Texo.Core.Configuration;

namespace BeaverSoft.Texo.Core.Inputting.Tree
{
    public class ParameterNode : BaseNode
    {
        public ParameterNode(Parameter parameter)
        {
            Parameter = parameter;
        }

        public Parameter Parameter { get; }

        public override NodeTypeEnum Type => NodeTypeEnum.Parameter;

        public override string ToString()
        {
            return $"Parameter: {Parameter.Key}";
        }
    }
}
