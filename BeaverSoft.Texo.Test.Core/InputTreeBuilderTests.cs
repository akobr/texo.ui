using BeaverSoft.Texo.Core;
using BeaverSoft.Texo.Core.Input.InputTree;
using StrongBeaver.Core.Services.Logging;
using Xunit;

namespace BeaverSoft.Texo.Test.Core
{
    public class InputTreeBuilderTests
    {
        [Fact]
        public void InputTree_BuildCurrentDirectory_CurrentDirectoryTree()
        {
            InputTreeBuilder builder = new InputTreeBuilder(new EmptyLogService());
            InputTree tree = builder.Build(new[] { CommandsBuilder.BuildCurrentDirectory() }, null);

            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.Equal(4, tree.Root.Queries.Count);
            Assert.Null(tree.Root.DefaultQuery);
            Assert.Single(tree.Root.Queries["cd"].Parameters);
        }

        [Fact]
        public void InputTree_BuildTexo_TexoTree()
        {
            InputTreeBuilder builder = new InputTreeBuilder(new EmptyLogService());
            InputTree tree = builder.Build(new[] { CommandsBuilder.BuildTexo() }, null);

            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.NotNull(tree.Root.Queries["texo"]);
            Assert.Equal(6, tree.Root.Queries["texo"].Queries.Count);
        }

        [Fact]
        public void InputTree_BuildHelp_HelpTree()
        {
            InputTreeBuilder builder = new InputTreeBuilder(new EmptyLogService());
            InputTree tree = builder.Build(new[] { CommandsBuilder.BuildHelp() }, null);

            Assert.NotNull(tree);
            Assert.NotNull(tree.Root);
            Assert.NotNull(tree.Root.Queries["help"]);
            Assert.Empty(tree.Root.Queries["help"].Queries);
            Assert.Empty(tree.Root.Queries["help"].Options);
            Assert.Single(tree.Root.Queries["help"].Parameters);
        }
    }
}
