using System;
using System.Collections.Generic;
using System.Text;
using BeaverSoft.Texo.Core.Configuration;

namespace BeaverSoft.Texo.Core.Help
{
    public class CommandTextTreeBuilder
    {
        private readonly bool showTemplates;
        private StringBuilder result;

        public CommandTextTreeBuilder(bool showTemplates)
        {
            this.showTemplates = showTemplates;
        }

        public string BuildTree(Query command)
        {
            result = new StringBuilder();
            RenderQuery(command, 0, false, true);
            return result.ToString();
        }

        public void RenderQuery(Query query, ushort level, bool isDefault, bool isLast)
        {
            RenderBullet(level, isLast);

            string mainRepresentation = query.GetMainRepresentation();
            result.Append(mainRepresentation);
            result.Append(" [q");

            if (isDefault)
            {
                result.Append("D");
            }

            result.Append("] ");
            RenderRepresentation(query, mainRepresentation);
            result.AppendLine();

            for (int i = 0, last = query.Queries.Count - 1; i <= last; i++)
            {
                Query subQuery = query.Queries[i];
                RenderQuery(
                    subQuery,
                    (ushort)(level + 1),
                    string.Equals(query.DefaultQueryKey, subQuery.Key, StringComparison.OrdinalIgnoreCase),
                    i == last);
            }

            for (int i = 0, last = query.Options.Count - 1; i <= last; i++)
            {
                Option option = query.Options[i];
                RenderOption(option, (ushort)(level + 1), i == last);
            }

            for (int i = 0, last = query.Parameters.Count - 1; i <= last; i++)
            {
                Parameter parameter = query.Parameters[i];
                RenderParameter(parameter, (ushort)(level + 1), i == last);
            }
        }

        public void RenderOption(Option option, ushort level, bool isLast)
        {
            RenderBullet(level, isLast);
            char? listCharacter = option.GetListCharacter();

            if (listCharacter.HasValue)
            {
                result.Append("-");
                result.Append(listCharacter.Value);
            }

            string mainRepresentation = option.GetMainRepresentation();
            result.Append("--");
            result.Append(mainRepresentation);
            result.Append(" [o] ");

            if (listCharacter.HasValue)
            {
                RenderRepresentation(option, mainRepresentation, listCharacter.Value.ToString());
            }
            else
            {
                RenderRepresentation(option, mainRepresentation);
            }

            result.AppendLine();

            for (int i = 0, last = option.Parameters.Count - 1; i <= last; i++)
            {
                Parameter parameter = option.Parameters[i];
                RenderParameter(parameter, (ushort)(level + 1), i == last);
            }
        }

        public void RenderParameter(Parameter parameter, ushort level, bool isLast)
        {
            RenderBullet(level, isLast);
            result.Append(parameter.Key);
            result.Append(" [p");

            if (parameter.IsOptional)
            {
                result.Append(parameter.IsRepeatable ? "0..*]" : "0..1]");
            }
            else
            {
                result.Append(parameter.IsRepeatable ? "1..*]" : "1..1]");
            }

            if (showTemplates && !string.IsNullOrWhiteSpace(parameter.ArgumentTemplate))
            {
                result.Append($" /{parameter.ArgumentTemplate}/");
            }

            result.AppendLine();
        }

        private void RenderBullet(ushort level, bool isLast)
        {
            //if (level > 0)
            //{
            //    for (int i = 1; i < level; i++)
            //    {
            //        result.Append(' ', 2);
            //        result.Append('│');
            //    }
            //}

            result.Append(' ', level * 2);
            //result.Append(isLast ? '└' : '├');
            result.Append('└');
            result.Append(' ');
        }

        private void RenderRepresentation(InputStatement statement, params string[] exceptedRepresentations)
        {
            if (statement.Representations.Count <= exceptedRepresentations.Length)
            {
                return;
            }

            result.Append('(');

            HashSet<string> exceptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string excepted in exceptedRepresentations)
            {
                exceptions.Add(excepted);
            }

            foreach (string representation in statement.Representations)
            {
                if (exceptions.Contains(representation))
                {
                    continue;
                }

                result.Append(representation);
                result.Append(", ");
            }

            result.Remove(result.Length - 2, 2);
            result.Append(')');
        }
    }
}
