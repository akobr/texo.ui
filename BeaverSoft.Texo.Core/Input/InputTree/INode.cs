namespace BeaverSoft.Texo.Core.Input.InputTree
{
    public interface INode
    {
        INode Parent { get; }

        NodeTypeEnum Type { get; }
    }
}