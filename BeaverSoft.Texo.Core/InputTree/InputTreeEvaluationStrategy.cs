using System.Collections.Generic;
using System.Text.RegularExpressions;
using BeaverSoft.Texo.Core.Commands;
using BeaverSoft.Texo.Core.Environment;
using BeaverSoft.Texo.Core.Input;
using BeaverSoft.Texo.Core.Model.Configuration;

namespace BeaverSoft.Texo.Core.InputTree
{
    public class InputTreeEvaluationStrategy : IInputTreeEvaluationStrategy
    {
        private readonly IEnvironmentService environment;
        private readonly InputTree tree;

        private IParsedInput input;
        private Input.Input.Builder result;
        private Stack<AnalysedToken> tokenStack;
        private int parameterIndex;
        private bool wrongInput;

        public InputTreeEvaluationStrategy(InputTree tree, IEnvironmentService environment)
        {
            this.environment = environment;
            this.tree = tree;
        }

        public IInput Evaluate(IParsedInput parsedInput)
        {
            input = parsedInput;

            if (IsInputNullOrEmpty())
            {
                return Input.Input.BuildUnrecognised(input);
            }

            PrepareTokenStack();

            wrongInput = false;
            result = Input.Input.Empty.ToBuilder();
            result.ParsedInput = input;
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
            int queryInputIndex = 0;

            while (!wrongInput && tokenStack.Count > 0)
            {
                AnalysedToken token = tokenStack.Peek();

                if (token.IsParameter)
                {
                    EvaluateParameters(query, result.Context.Parameters);
                }
                else if (token.IsExplicitOption)
                {
                    EvaluateOption(query);
                }
                else if (query.Queries.TryGetValue(token.NormalisedValue, out QueryNode subQuery))
                {
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

        private void EvaluateParameters(ParameteriseNode statement, IDictionary<string, IParameterContext> context)
        {
            parameterIndex = 0;
            ParameterContext.Builder repeatableContext = null;

            while (!wrongInput
                   && parameterIndex < statement.Parameters.Count
                   && tokenStack.Count > 0)
            {
                AnalysedToken token = tokenStack.Peek();
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
                        if (repeatableContext == null)
                        {
                            repeatableContext = ParameterContext.Empty.ToBuilder();
                            repeatableContext.Key = parameter.Parameter.Key;
                        }

                        repeatableContext.Values.Add(token.Value);
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
                        if (repeatableContext == null)
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
                        if (repeatableContext == null)
                        {
                            repeatableContext = ParameterContext.Empty.ToBuilder();
                            repeatableContext.Key = parameter.Parameter.Key;
                        }

                        repeatableContext.Values.Add(token.Value);
                    }
                    else
                    {
                        context[parameter.Parameter.Key] = ParameterContext.Build(parameter.Parameter.Key, token.Value);
                        parameterIndex++;
                    }
                }
            }

            if (repeatableContext != null)
            {
                context[repeatableContext.Key] = repeatableContext.ToImmutable();
            }
        }

        private void PrepareTokenStack()
        {
            tokenStack = new Stack<AnalysedToken>();

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

        private void AddUnknownTokenToResult(string tokenInput)
        {
            result.Tokens.Add(new Token(TokenTypeEnum.Unknown, tokenInput, tokenInput));
        }

        private bool IsInputNullOrEmpty()
        {
            return input?.Tokens == null || input.Tokens.Count < 1;
        }

        private static bool IsParameterMatch(ParameterNode parameter, AnalysedToken token)
        {
            if (string.IsNullOrEmpty(parameter.Parameter.ArgumentTemplate))
            {
                return true;
            }

            return Regex.IsMatch(token.Value, parameter.Parameter.ArgumentTemplate);
        }
    }
}
