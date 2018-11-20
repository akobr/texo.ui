using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.VisualStudio.LanguageServices;

namespace Commands.CodeBaseSearch
{
    public class CodeBaseSearchService
    {
        private const string solutionPath = @"C:\Working\textum.ui\BeaverSoft.Texo.sln";

        public CodeBaseSearchService()
        {
            MSBuildWorkspace msWorkspace = MSBuildWorkspace.Create();
            Solution solution = msWorkspace.OpenSolutionAsync(solutionPath).Result;

            foreach (Project project in solution.Projects)
            {
                if (string.IsNullOrEmpty(project.FilePath)
                    || (!string.Equals(project.Language, "csharp", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(project.Language, "c#", StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                foreach (Document document in project.Documents)
                {
                    if (document.SourceCodeKind != SourceCodeKind.Regular)
                    {
                        continue;
                    }

                    //document.TryGetSyntaxTree();
                    //document.GetSyntaxTreeAsync();
                    //document.GetSyntaxVersionAsync();
                }
            }

        }
    }
}
