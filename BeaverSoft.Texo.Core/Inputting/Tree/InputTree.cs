namespace BeaverSoft.Texo.Core.Inputting.Tree
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
