using BeaverSoft.Texo.Core.Configuration;

namespace BeaverSoft.Texo.Core.Inputting.InputTree
{
    public class OptionNode : ParameteriseNode
    {
        public OptionNode(Option option)
        {
            Option = option;
        }

        public Option Option { get; set; }

        public override NodeTypeEnum Type => NodeTypeEnum.Option;

        public override string ToString()
        {
            return $"Option: {Option.Key}";
        }
    }
}
