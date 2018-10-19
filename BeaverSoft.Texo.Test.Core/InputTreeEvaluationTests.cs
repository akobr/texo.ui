using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.InputTree;
using BeaverSoft.Texo.Core.Model.Configuration;
using StrongBeaver.Core.Services.Logging;
using Xunit;

namespace BeaverSoft.Texo.Test.Core
{
    public class InputTreeEvaluationTests
    {
        [Fact]
        public void TreeEvaluation_ConfigurationCommand_EvaluateInput()
        {
            const string input = "cd ../folder '../second folder/sub'";

            var cdBuilder = Query.CreateBuilder();

            var cdParamBuilder = Parameter.CreateBuilder();
            cdParamBuilder.Key = "path";
            cdParamBuilder.ArgumentTemplate = "[^\\<\\>\\:\\\"\\|\\?\\*[:cntrl:]]+";
            cdParamBuilder.IsOptional = true;
            cdParamBuilder.IsRepeatable = true;
            cdParamBuilder.Documentation.Title = "Directory path";
            cdParamBuilder.Documentation.Description = "Specify full or relative path.";

            cdBuilder.Key = "current-directory";
            cdBuilder.Representations.AddRange(new[] { "current-directory", "cd", "chdir", "cdir" });
            cdBuilder.Parameters.Add(cdParamBuilder.ToImmutable());
            cdBuilder.Documentation.Title = "Current directory";
            cdBuilder.Documentation.Description = "Gets or sets current working directory.";

            InputTreeBuilder builder = new InputTreeBuilder(new EmptyLogService());
            InputTree tree = builder.Build(new[] { cdBuilder.ToImmutable() }, null);

            IEnvironmentService environment = new EnvironmentService();
            IInputParseService parser = new InputParseService();
            InputTreeEvaluationStrategy evaluation = new InputTreeEvaluationStrategy(tree, environment);

            IParsedInput parsed = parser.Parse(input);
            IInput evaluated = evaluation.Evaluate(parsed);

            Assert.Equal(parsed, evaluated.ParsedInput);
            Assert.Equal(3, evaluated.Tokens.Count);
            Assert.True(evaluated.Context.IsValid);
            Assert.Equal("current-directory", evaluated.Context.Key);
            Assert.Empty(evaluated.Context.Options);
            Assert.Single(evaluated.Context.Parameters);
            Assert.Equal("../folder", evaluated.Context.Parameters["path"].GetValues()[0]);
            Assert.Equal("../second folder/sub", evaluated.Context.Parameters["path"].GetValues()[1]);
        }
    }
}
