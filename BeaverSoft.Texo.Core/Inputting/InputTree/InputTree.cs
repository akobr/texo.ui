namespace BeaverSoft.Texo.Core.Inputting.InputTree
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
