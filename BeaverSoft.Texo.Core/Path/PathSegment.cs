using System;
using System.Text;
using System.Text.RegularExpressions;

namespace BeaverSoft.Texo.Core.Path
{
    public class PathSegment
    {
        public PathSegment(string segment)
        {
            Value = segment ?? throw new ArgumentNullException(nameof(segment));
            WildcardType = ProcessWildcards(segment, out string regularExpression);
            RegularExpression = regularExpression;
        }

        public string Value { get; }

        public WildcardTypeEnum WildcardType { get; }

        public string RegularExpression { get; }

        private static WildcardTypeEnum ProcessWildcards(string segment, out string regularExpression)
        {
            StringBuilder regexBuilder = new StringBuilder();
            StringBuilder tokenBuilder = new StringBuilder(segment.Length);
            bool simpleWidlcard = false;
            bool complexWildcard = false;

            for (int i = 0; i < segment.Length; i++)
            {
                char character = segment[i];

                if (character.IsPathWildcard())
                {
                    if (tokenBuilder.Length > 0)
                    {
                        regexBuilder.Append(Regex.Escape(tokenBuilder.ToString()));
                        tokenBuilder.Clear();
                    }

                    if (character == PathConstants.WILDCARD_ANY_CHARACTER
                        && i < segment.Length - 1
                        && segment[i + 1] == PathConstants.WILDCARD_ANY_CHARACTER)
                    {
                        regexBuilder.Append(".*");
                        complexWildcard = true;
                        i++;
                        continue;
                    }

                    regexBuilder.Append(@"[^\\\/]");
                    regexBuilder.Append(character);
                    simpleWidlcard = true;
                }
                else
                {
                    tokenBuilder.Append(character);
                }
            }

            if (tokenBuilder.Length > 0)
            {
                regexBuilder.Append(Regex.Escape(tokenBuilder.ToString()));
            }

            regularExpression = regexBuilder.ToString();

            return complexWildcard
                ? WildcardTypeEnum.Complex
                : (simpleWidlcard ? WildcardTypeEnum.Simple : WildcardTypeEnum.None);
        }

        public enum WildcardTypeEnum
        {
            None = 0,
            Simple,
            Complex
        }
    }
}
