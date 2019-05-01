namespace BeaverSoft.Texo.Core.Inputting.Tree
{
    public interface INode
    {
        INode Parent { get; }

        NodeTypeEnum Type { get; }
    }
}