using BeaverSoft.Texo.Core.InputTree;
using BeaverSoft.Texo.Core.Model.Configuration;
using StrongBeaver.Core.Services.Logging;
using Xunit;

namespace BeaverSoft.Texo.Test.Core
{
    public class InputTreeBuilderTests
    {
        [Fact]
        public void InputTree_CurrentDirectoryDefinition_CurrentDirectoryTree()
        {
            var cdBuilder = Query.CreateBuilder();

            var cdParamBuilder = Parameter.CreateBuilder();
            cdParamBuilder.Key = "path";
            cdParamBuilder.ArgumentTemplate = "[^\\<\\>\\:\\\"\\|\\?\\*[:cntrl:]]+";
            cdParamBuilder.IsOptional = true;
            cdParamBuilder.IsRepeatable = true;
            cdParamBuilder.Documentation = new Documentation("Directory path", "Specify full or relative path.");

            cdBuilder.Key = "current-directory";
            cdBuilder.Representations.AddRange(new[] { "current-directory", "cd", "chdir", "cdir"});
            cdBuilder.Parameters.Add(cdParamBuilder.ToImmutable());
            cdBuilder.Documentation = new Documentation("Current directory", "Gets or sets current working directory.");

            InputTreeBuilder builder = new InputTreeBuilder(new EmptyLogService());
            InputTree tree = builder.Build(new[] {cdBuilder.ToImmutable()}, null);

            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.Equal(4, tree.Root.Queries.Count);
            Assert.Null(tree.Root.DefaultQuery);
            Assert.Single(tree.Root.Queries["cd"].Parameters);
        }
    }
}
