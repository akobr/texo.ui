namespace BeaverSoft.Texo.Core.Input.InputTree
{
    public class InputTree
    {
        public InputTree()
        {
            Root = new QueryNode(null);
        }

        public QueryNode Root { get; }
    }
}
