using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BeaverSoft.Texo.Core.Path
{
    public class TexoPath : IEquatable<TexoPath>
    {
        public const char WILDCARD_ONE_CHARACTER = '?';
        public const char WILDCARD_ANY_CHARACTER = '*';
        public const string WILDCARD_ANY_PATH = "**";

        private List<int> wildcardIndexes;
        private Regex regularExpression;

        public TexoPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path can't be null or empty.", nameof(path));
            }

            Path = path;
            IsRelative = path.IsRelative();
            ContainsWildcard = BuildSegments();

            if (ContainsWildcard)
            {
                return;
            }

            try
            {
                AbsolutePath = System.IO.Path.GetFullPath(path);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid path.", nameof(path), ex);
            }
        }

        public string Path { get; }

        public string AbsolutePath { get; }

        public bool ContainsWildcard { get; }

        public bool IsAbsolute => !IsRelative;

        public bool IsRelative { get; }

        public IImmutableList<PathSegment> Segments { get; private set; }

        public IImmutableList<string> GetItems()
        {
            List<string> items = new List<string>
            {
                BuildInitialPath(out int wildcardIndex)
            };

            items = ProcessItems(items, wildcardIndex);
            return items.ToImmutableList();
        }

        public IImmutableList<string> GetFiles()
        {
            List<string> items = new List<string>
            {
                BuildInitialPath(out int wildcardIndex)
            };

            items = ProcessItems(items, wildcardIndex);
            return items.Where(File.Exists).ToImmutableList();
        }

        public IImmutableList<string> GetDirectories()
        {
            List<string> items = new List<string>
            {
                BuildInitialPath(out int wildcardIndex)
            };

            items = ProcessItems(items, wildcardIndex);
            return items.Where(Directory.Exists).ToImmutableList();
        }

        public bool Equals(TexoPath other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((TexoPath) obj);
        }

        public override int GetHashCode()
        {
            if (ContainsWildcard)
            {
                return Path == null
                    ? 0
                    : StringComparer.OrdinalIgnoreCase.GetHashCode(Path);
            }

            return AbsolutePath == null
                ? 0
                : StringComparer.OrdinalIgnoreCase.GetHashCode(AbsolutePath);
        }

        private List<string> ProcessItems(List<string> directories, int wildcardIndex)
        {
            while (true)
            {
                PathSegment wildcard = Segments[wildcardIndex];

                if (wildcard.WildcardType == PathSegment.WildcardTypeEnum.Complex)
                {
                    return ProcessComplexItems(directories);
                }

                if (wildcardIndex == Segments.Count - 1)
                {
                    return ProcessLeafItems(directories, wildcard);
                }

                List<string> newDirectories = new List<string>();

                foreach (string directory in directories)
                {
                    if (wildcard.WildcardType == PathSegment.WildcardTypeEnum.Simple)
                    {
                        newDirectories.AddRange(Directory.GetDirectories(directory, wildcard.Value, SearchOption.TopDirectoryOnly));
                    }
                    else
                    {
                        string newDirectory = System.IO.Path.Combine(directory, wildcard.Value);
                        if (Directory.Exists(newDirectory))
                        {
                            newDirectories.Add(newDirectory);
                        }
                    }
                }

                directories = newDirectories;
                wildcardIndex++;
            }
        }

        private List<string> ProcessComplexItems(List<string> directories)
        {
            List<string> newItems = new List<string>();

            foreach (string directory in directories)
            {
                string[] subItems = Directory.GetFileSystemEntries(
                    directory + System.IO.Path.DirectorySeparatorChar, string.Empty, SearchOption.AllDirectories);

                newItems.AddRange(subItems.Where(item => regularExpression.IsMatch(item)));
            }

            return newItems;
        }

        private bool BuildSegments()
        {
            StringBuilder regexBuilder = new StringBuilder();
            wildcardIndexes = new List<int>();

            var segments = ImmutableList<PathSegment>.Empty.ToBuilder();
            bool wildcard = false;
            int index = 0;

            foreach (string strSegment in Path.SplitToSegments())
            {
                PathSegment segment = new PathSegment(strSegment);
                segments.Add(segment);

                if (segment.WildcardType != PathSegment.WildcardTypeEnum.None)
                {
                    wildcardIndexes.Add(index);
                    wildcard = true;
                }

                index++;
                regexBuilder.Append(segment.RegularExpression);
                regexBuilder.Append(@"[\\\/]+");
            }

            regexBuilder.Remove(regexBuilder.Length - 1, 1);
            regexBuilder.Append("*$");

            Segments = segments.ToImmutable();
            regularExpression = new Regex(regexBuilder.ToString(), RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return wildcard;
        }

        private string BuildInitialPath(out int segmentIndex)
        {
            string path = IsRelative ? System.IO.Path.GetFullPath(".") : string.Empty;
            segmentIndex = ContainsWildcard ? wildcardIndexes.First() : Segments.Count - 1;

            for (int i = 0; i < segmentIndex; i++)
            {
                PathSegment segment = Segments[i];
                path = System.IO.Path.Combine(path, segment.Value);
            }

            return path;
        }
        private static List<string> ProcessLeafItems(List<string> directories, PathSegment leafSegment)
        {
            List<string> newItems = new List<string>();

            foreach (string directory in directories)
            {
                newItems.AddRange(Directory.GetFileSystemEntries(
                    directory, leafSegment.Value, SearchOption.TopDirectoryOnly));
            }

            return newItems;
        }
    }
}
