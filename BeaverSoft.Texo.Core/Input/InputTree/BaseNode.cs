namespace BeaverSoft.Texo.Core.Input.InputTree
{
    public abstract class BaseNode : INode
    {
        public INode Parent { get; private set; }

        public abstract NodeTypeEnum Type { get; }

        public void SetParent(INode parent)
        {
            Parent = parent;
        }
    }
}
