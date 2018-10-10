using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.InputTree
{
    public class OptionNode : ParameteriseNode
    {
        public OptionNode(IOption option)
        {
            Option = option;
        }

        public IOption Option { get; set; }

        public override NodeTypeEnum Type => NodeTypeEnum.Option;
    }
}
