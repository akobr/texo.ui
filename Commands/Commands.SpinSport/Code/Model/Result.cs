using System;
using System.Collections.Generic;

namespace Commands.SpinSport.Code.Model
{
    public class Result
    {
        public List<Record> Records { get; set; }

        public string SearchTerm { get; set; }

        public IReadOnlyCollection<string> SearchWords { get; set; }

        public TimeSpan SearchTime { get; set; }
    }
}
