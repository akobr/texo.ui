using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace BeaverSoft.Texo.Commands.FileManager.Path
{
    public class WildcardPath
    {
        public const char WILDCARD_ONE_CHARACTER = '?';
        public const char WILDCARD_ANY_CHARACTER = '*';
        public const string WILDCARD_ANY_PATH = "**";

        private readonly List<Segment> segments;

        public WildcardPath(string path)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path can't be null or empty.", nameof(path));
            }

            segments = new List<Segment>();
            Path = path;
            IsRelative = path.IsRelative();

            BuildSegments();
        }

        public string Path { get; }

        public bool ContainsWidlcard { get; private set; }

        public bool IsRelative { get; private set; }

        public IImmutableList<string> GetFiles(string currentDirectory = null)
        {
            if (string.IsNullOrEmpty(currentDirectory)
                && IsRelative)
            {
                return ImmutableList<string>.Empty;
            }
        }

        public IImmutableList<string> GetDirectories(string currentDirectory = null)
        {

        }

        private void BuildSegments()
        {
            bool wildcard = false;

            foreach (string strSegment in Path.SplitToSegments())
            {
                Segment segment = new Segment(strSegment);
                segments.Add(segment);

                if (segment.ContainsWildcard)
                {
                    wildcard = true;
                }
            }

            ContainsWidlcard = wildcard;
        }
    }
}
