using System.Collections.Generic;
using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.InputTree
{
    public class QueryNode : ParameteriseNode
    {
        public QueryNode(Query query)
        {
            Query = query;

            Queries = new Dictionary<string, QueryNode>();
            Options = new Dictionary<string, OptionNode>();
            OptionList = new Dictionary<char, OptionNode>();
        }

        public Query Query { get; }

        public override NodeTypeEnum Type => NodeTypeEnum.Query;

        public QueryNode DefaultQuery { get; private set; }

        public Dictionary<string, QueryNode> Queries { get; }

        public Dictionary<string, OptionNode> Options { get; }

        public Dictionary<char, OptionNode> OptionList { get; }

        public void SetDefaultQuery(QueryNode defaultQuery)
        {
            DefaultQuery = defaultQuery;
        }
    }

}