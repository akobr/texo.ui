namespace BeaverSoft.Texo.Core.Inputting.InputTree
{
    public interface INode
    {
        INode Parent { get; }

        NodeTypeEnum Type { get; }
    }
}