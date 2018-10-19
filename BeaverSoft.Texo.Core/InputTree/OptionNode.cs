using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.InputTree
{
    public class OptionNode : ParameteriseNode
    {
        public OptionNode(Option option)
        {
            Option = option;
        }

        public Option Option { get; set; }

        public override NodeTypeEnum Type => NodeTypeEnum.Option;
    }
}
