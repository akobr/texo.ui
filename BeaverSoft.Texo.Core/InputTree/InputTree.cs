namespace BeaverSoft.Texo.Core.InputTree
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
