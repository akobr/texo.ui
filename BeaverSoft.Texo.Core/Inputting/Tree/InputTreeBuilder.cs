using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Core.Configuration;
using StrongBeaver.Core.Services.Logging;

namespace BeaverSoft.Texo.Core.Inputting.Tree
{
    public class InputTreeBuilder : IInputTreeBuilder
    {
        private readonly ILogService logger;

        public InputTreeBuilder(ILogService logger)
        {
            this.logger = logger;
        }

        public InputTree Build(IEnumerable<Query> commands, string defaultCommandKey)
        {
            InputTree tree = new InputTree();
            ProcessQueries(tree.Root, commands, defaultCommandKey);
            return tree;
        }

        private QueryNode BuildQueryNode(Query query)
        {
            QueryNode node = new QueryNode(query);
            ProcessQueries(node, query.Queries, query.DefaultQueryKey);
            ProcessOptions(node, query.Options);
            ProcessParameters(node, query.Parameters);
            return node;
        }

        private OptionNode BuildOptionNode(Option option)
        {
            OptionNode node = new OptionNode(option);
            ProcessParameters(node, option.Parameters);
            return node;
        }

        private static ParameterNode BuildParameterNode(Parameter parameter)
        {
            return new ParameterNode(parameter);
        }

        private void AddQuery(QueryNode query, QueryNode parent)
        {
            foreach (string representation in query.Query.Representations)
            {
                if (!IsValidRepresentation(representation))
                {
                    continue;
                }

                string normalisedRepresentation = NormaliseInput(representation);

                if (parent.Queries.ContainsKey(normalisedRepresentation))
                {
                    logger.Error("The query already contains sub-query with same representation.", parent.Query,
                        query.Query, representation);
                    continue;
                }

                parent.Queries.Add(normalisedRepresentation, query);
            }
        }

        private void AddOption(OptionNode option, QueryNode parent)
        {
            foreach (string representation in option.Option.Representations)
            {
                if (!IsValidRepresentation(representation))
                {
                    continue;
                }

                string normalisedRepresentation = NormaliseInput(representation);

                if (parent.Queries.ContainsKey(normalisedRepresentation))
                {
                    logger.Error("The query already contains sub-query with same representation.", parent.Query,
                        option.Option, representation);
                    continue;
                }

                if (parent.Options.ContainsKey(normalisedRepresentation))
                {
                    logger.Error("The query already contains option with same representation.", parent.Query,
                        option.Option, representation);
                    continue;
                }

                parent.Options.Add(normalisedRepresentation, option);
            }
        }

        private void AddParameter(ParameterNode parameterNode, ParameteriseNode node)
        {
            if (!CheckIfParameterIsValid(parameterNode, node))
            {
                return;
            }

            node.Parameters.Add(parameterNode);
        }

        private void ProcessQueries(QueryNode parent, IEnumerable<Query> queries, string defaultQueryKey)
        {
            foreach (Query query in queries)
            {
                QueryNode node = BuildQueryNode(query);
                AddQuery(node, parent);

                if (EqualKey(defaultQueryKey, query.Key))
                {
                    parent.SetDefaultQuery(node);
                }
            }
        }

        private void ProcessOptions(QueryNode parent, IEnumerable<Option> options)
        {
            foreach (Option option in options)
            {
                OptionNode optionNode = BuildOptionNode(option);
                AddOption(optionNode, parent);
            }
        }

        private void ProcessParameters(ParameteriseNode parent, IEnumerable<Parameter> parameters)
        {
            foreach (Parameter parameter in parameters)
            {
                ParameterNode parameterNode = BuildParameterNode(parameter);
                AddParameter(parameterNode, parent);
            }
        }

        private bool CheckIfParameterIsValid(ParameterNode parameterNode, ParameteriseNode node)
        {
            Parameter newParameter = parameterNode.Parameter;

            if (NeedsTemplate(newParameter)
                && !IsValidTemplate(newParameter.ArgumentTemplate))
            {
                logger.Error("Any optional or repeateble parameter needs to have a valid template.", GetContextFromNode(node), newParameter);
                return false;
            }

            if (node.Parameters.Count < 1)
            {
                return true;
            }

            Parameter lastParameter = node.Parameters[node.Parameters.Count - 1].Parameter;

            if (lastParameter.IsRepeatable)
            {
                logger.Error("A repeateble parameter needs to be the last parameter.", GetContextFromNode(node), lastParameter, newParameter);
                return false;
            }

            if (!newParameter.IsOptional
                && lastParameter.IsOptional)
            {
                logger.Error("Required parameters need to be defined before optional.", GetContextFromNode(node), newParameter);
                return false;
            }

            if (node.Type == NodeTypeEnum.Option
                && node.Parameters.Count >= 2)
            {
                logger.Warn("An option shouldn't have more than two parameters.", ((OptionNode)node).Option);
            }
            else if (node.Type == NodeTypeEnum.Query
                     && node.Parameters.Count >= 3)
            {
                logger.Warn("An option shouldn't have more than three parameters.", ((QueryNode)node).Query);
            }

            return true;
        }

        private static object GetContextFromNode(INode node)
        {
            switch (node.Type)
            {
                case NodeTypeEnum.Query:
                    return ((QueryNode) node).Query;

                case NodeTypeEnum.Option:
                    return ((OptionNode) node).Option;

                case NodeTypeEnum.Parameter:
                    return ((ParameterNode) node).Parameter;

                default:
                    return null;
            }
        }

        private string NormaliseInput(string input)
        {
            StringBuilder normaliseBuilder = new StringBuilder(input);

            int startIndex, endIndex;

            for (startIndex = 0; startIndex < input.Length; startIndex++)
            {
                char character = input[startIndex];

                if (!char.IsWhiteSpace(character)
                    && character != InputConstants.PARAMETER_INPUT_PREFIX)
                {
                    break;
                }
            }

            for (endIndex = input.Length - 1; endIndex >= 0; endIndex--)
            {
                char character = input[endIndex];

                if (!char.IsWhiteSpace(character)
                    && character != InputConstants.PARAMETER_INPUT_PREFIX)
                {
                    break;
                }
            }

            string result = normaliseBuilder.ToString(startIndex, endIndex + 1 - startIndex);
            result = InputRegex.Reserved.Replace(result, string.Empty);

            if (result.Length != input.Length)
            {
                logger.Warn("An input definition contains reserved character(s).", input);
            }

            return result.ToLowerInvariant();
        }

        private bool IsValidRepresentation(string representation)
        {
            if (string.IsNullOrEmpty(representation))
            {
                logger.Error("An input representation is null or empty.", representation);
                return false;
            }

            if (InputRegex.WhiteSpaces.IsMatch(representation))
            {
                logger.Error("An input representation contains white space(s).", representation);
                return false;
            }

            if (representation[0] == InputConstants.PARAMETER_INPUT_PREFIX)
            {
                logger.Error("An input representation starts with the option prefix.", representation);
                return false;
            }

            return true;
        }

        private static bool EqualKey(string firstKey, string secondKey)
        {
            return string.Equals(firstKey, secondKey, StringComparison.InvariantCulture);
        }

        private static bool NeedsTemplate(Parameter parameter)
        {
            return parameter.IsOptional || parameter.IsRepeatable;
        }

        private static bool IsValidTemplate(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            try
            {
                Regex.Match(string.Empty, pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
    }
}