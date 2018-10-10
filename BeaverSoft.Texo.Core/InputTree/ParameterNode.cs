using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.InputTree
{
    public class ParameterNode : BaseNode
    {
        public ParameterNode(IParameter parameter)
        {
            Parameter = parameter;
        }

        public IParameter Parameter { get; }

        public override NodeTypeEnum Type => NodeTypeEnum.Parameter;
    }
}
