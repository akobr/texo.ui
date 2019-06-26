using System.Collections.Generic;

namespace Commands.CodeBaseSearch
{
    public class SearchContext
    {
        public string SearchTerm { get; set; }

        public ISet<string> Categories { get; set; }
    }
}
