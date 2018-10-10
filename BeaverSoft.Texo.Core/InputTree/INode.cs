namespace BeaverSoft.Texo.Core.InputTree
{
    public interface INode
    {
        INode Parent { get; }

        NodeTypeEnum Type { get; }
    }
}