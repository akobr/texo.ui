using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Configuration;
using BeaverSoft.Texo.Core.Environment;

namespace BeaverSoft.Texo.Core.Input.InputTree
{
    public class InputTreeEvaluationStrategy : IInputTreeEvaluationStrategy
    {
        private readonly IEnvironmentService environment;
        private readonly InputTree tree;

        private ParsedInput input;
        private Core.Input.Input.Builder result;
        private Stack<AnalysedToken> tokenStack;
        private int parameterIndex;
        private bool wrongInput;

        public InputTreeEvaluationStrategy(InputTree tree, IEnvironmentService environment)
        {
            this.tree = tree ?? throw new ArgumentNullException(nameof(tree));
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public Core.Input.Input Evaluate(ParsedInput parsedInput)
        {
            input = parsedInput;

            if (IsInputNullOrEmpty())
            {
                return Core.Input.Input.BuildUnrecognised(input);
            }

            PrepareTokenStack();

            wrongInput = false;
            result = Core.Input.Input.Empty.ToBuilder();
            result.ParsedInput = input.ToBuilder();
            AnalysedToken firstToken = tokenStack.Peek();

            if (firstToken.IsParameter || firstToken.IsExplicitOption)
            {
                if (tree.Root.DefaultQuery == null)
                {
                    AddWrongTokenToResult(firstToken.RawInput);
                }
                else
                {
                    result.Context.Key = tree.Root.DefaultQuery.Query.Key;
                    EvaluateQueueBody(tree.Root.DefaultQuery);
                }
            }
            else if (tree.Root.Queries.TryGetValue(firstToken.NormalisedValue, out QueryNode command))
            {
                result.Context.Key = command.Query.Key;
                EvaluateExplicitQueue(command);
            }
            else
            {
                AddWrongTokenToResult(firstToken.RawInput);
            }

            FinishEvaluation();
            return result.ToImmutable();
        }

        private void FinishEvaluation()
        {
            result.Context.IsValid = !wrongInput;

            if (!wrongInput)
            {
                return;
            }

            tokenStack.Pop();
            while (tokenStack.Count > 0)
            {
                AddWrongTokenToResult(tokenStack.Pop().RawInput);
            }
        }

        private void EvaluateExplicitQueue(QueryNode query)
        {
            AnalysedToken token = tokenStack.Pop();
            AddTokenToResult(TokenTypeEnum.Query, token.RawInput, query.Query.GetMainRepresentation());

            EvaluateQueueBody(query);
        }

        private void EvaluateQueueBody(QueryNode query)
        {
            if (query.DefaultQuery != null)
            {
                EvaluateDefaultQuery(query);
                return;
            }

            int queryInputIndex = 0;

            while (!wrongInput && tokenStack.Count > 0)
            {
                AnalysedToken token = tokenStack.Peek();

                if (token.IsExplicitOption)
                {
                    EvaluateOption(query);
                }
                else if (token.CanBeQuery && query.Queries.TryGetValue(token.NormalisedValue, out QueryNode subQuery))
                {
                    result.Context.QueryPath.Add(subQuery.Query.Key);
                    EvaluateExplicitQueue(subQuery);
                }
                else if(queryInputIndex == 0)
                {
                    EvaluateOptionList(query);
                }
                else
                {
                    EvaluateParameters(query, result.Context.Parameters);
                }

                queryInputIndex++;
            }
        }

        private void EvaluateDefaultQuery(QueryNode query)
        {
            AnalysedToken token = tokenStack.Peek();

            if (token.CanBeQuery && query.Queries.TryGetValue(token.NormalisedValue, out QueryNode subQuery))
            {
                result.Context.QueryPath.Add(subQuery.Query.Key);
                EvaluateExplicitQueue(subQuery);
            }
            else
            {
                result.Context.QueryPath.Add(query.DefaultQuery.Query.Key);
                EvaluateQueueBody(query.DefaultQuery);
            }
        }

        private void EvaluateOption(QueryNode query)
        {
            AnalysedToken token = tokenStack.Peek();

            if (!query.Options.TryGetValue(token.NormalisedValue, out OptionNode option))
            {
                EvaluateOptionList(query);
                return;
            }

            OptionContext.Builder optionContext = OptionContext.Empty.ToBuilder();
            optionContext.Key = option.Option.Key;

            AddTokenToResult(TokenTypeEnum.Option, token.RawInput, option.Option.GetMainRepresentation());
            tokenStack.Pop();
            EvaluateParameters(option, optionContext.Parameters);

            result.Context.Options.Add(optionContext.Key, optionContext.ToImmutable());
        }

        private void EvaluateOptionList(QueryNode query)
        {
            AnalysedToken token = tokenStack.Peek();
            List<OptionNode> options = new List<OptionNode>();

            foreach (char character in token.NormalisedValue)
            {
                if (character == InputConstants.PARAMETER_INPUT_PREFIX)
                {
                    continue;
                }

                if (!query.OptionList.TryGetValue(character, out OptionNode option))
                {
                    EvaluateParameters(query, result.Context.Parameters);
                    return;
                }

                options.Add(option);
            }

            foreach (OptionNode option in options)
            {
                result.Context.Options.Add(option.Option.Key, OptionContext.BuildWithoutParameters(option.Option.Key));
            }

            AddTokenToResult(TokenTypeEnum.OptionList, token.RawInput);
        }

        private void EvaluateParameters(ParameteriseNode statement, IDictionary<string, ParameterContext> context)
        {
            parameterIndex = 0;
            AnalysedToken token = null;
            ParameterContext.Builder repetableParameter = null;

            while (!wrongInput
                   && parameterIndex < statement.Parameters.Count
                   && tokenStack.Count > 0)
            {
                token = tokenStack.Peek();
                ParameterNode parameter = statement.Parameters[parameterIndex];

                if (token.IsEndOfParameterList)
                {
                    if (parameter.Parameter.IsRepeatable || parameter.Parameter.IsOptional)
                    {
                        AddTokenToResult(TokenTypeEnum.EndOfParameterList, token.RawInput);
                        tokenStack.Pop();
                        break;
                    }

                    AddWrongTokenToResult(token.RawInput);
                    return;
                }

                if (parameter.Parameter.IsOptional)
                {
                    if (!IsParameterMatch(parameter, token))
                    {
                        parameterIndex++;
                        continue;
                    }

                    AddTokenToResult(TokenTypeEnum.Parameter, token.RawInput, token.Value);
                    tokenStack.Pop();

                    if (parameter.Parameter.IsRepeatable)
                    {
                        if (repetableParameter == null)
                        {
                            repetableParameter = ParameterContext.Empty.ToBuilder();
                            repetableParameter.Key = parameter.Parameter.Key;
                        }

                        repetableParameter.Values.Add(token.Value);
                    }
                    else
                    {
                        context[parameter.Parameter.Key] = ParameterContext.Build(parameter.Parameter.Key, token.Value);
                        parameterIndex++;
                    }
                }
                else
                {
                    if (!IsParameterMatch(parameter, token))
                    {
                        if (repetableParameter == null)
                        {
                            AddWrongTokenToResult(token.RawInput);
                            return;
                        }

                        break;
                    }

                    AddTokenToResult(TokenTypeEnum.Parameter, token.RawInput, token.Value);
                    tokenStack.Pop();

                    if (parameter.Parameter.IsRepeatable)
                    {
                        if (repetableParameter == null)
                        {
                            repetableParameter = ParameterContext.Empty.ToBuilder();
                            repetableParameter.Key = parameter.Parameter.Key;
                        }

                        repetableParameter.Values.Add(token.Value);
                    }
                    else
                    {
                        context[parameter.Parameter.Key] = ParameterContext.Build(parameter.Parameter.Key, token.Value);
                        parameterIndex++;
                    }
                }
            }

            if (repetableParameter != null)
            {
                context[repetableParameter.Key] = repetableParameter.ToImmutable();
            }

            if (!wrongInput
                && statement.Type == NodeTypeEnum.Query)
            {
                if (tokenStack.Peek().IsEndOfInput)
                {
                    tokenStack.Pop();
                }

                if (tokenStack.Count > 0)
                {
                    if (token != null
                        && !token.IsEndOfParameterList)
                    {
                        AddWrongTokenToResult(token.RawInput);
                    }

                    if (statement.Parameters.Count < 1)
                    {
                        AddWrongTokenToResult(tokenStack.Peek().RawInput);
                    }
                }
            }
        }

        private void PrepareTokenStack()
        {
            tokenStack = new Stack<AnalysedToken>();
            tokenStack.Push(new AnalysedToken(string.Empty, environment));

            for (int i = input.Tokens.Count - 1; i >= 0; i--)
            {
                tokenStack.Push(new AnalysedToken(input.Tokens[i], environment));
            }
        }

        private void AddTokenToResult(TokenTypeEnum tokenType, string tokenInput)
        {
            result.Tokens.Add(new Token(tokenType, tokenInput, tokenInput));
        }

        private void AddTokenToResult(TokenTypeEnum tokenType, string tokenInput, string tokenTitle)
        {
            result.Tokens.Add(new Token(tokenType, tokenTitle, tokenInput));
        }

        private void AddWrongTokenToResult(string tokenInput)
        {
            result.Tokens.Add(new Token(TokenTypeEnum.Wrong, tokenInput, tokenInput));
            wrongInput = true;
        }

        private bool IsInputNullOrEmpty()
        {
            return input?.Tokens == null || input.Tokens.Count < 1;
        }

        private static bool IsParameterMatch(ParameterNode parameter, AnalysedToken token)
        {
            if (!token.IsEndOfInput
                && string.IsNullOrEmpty(parameter.Parameter.ArgumentTemplate))
            {
                return true;
            }

            return Regex.IsMatch(token.Value, parameter.Parameter.ArgumentTemplate);
        }
    }
}
