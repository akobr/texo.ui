using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BeaverSoft.Texo.Core.Path
{
    public partial class TexoPath : IEquatable<TexoPath>
    {
        private readonly string path;
        private List<int> wildcardIndexes;
        private Regex regularExpression;

        public TexoPath(Uri path)
            : this(path?.AbsolutePath)
        {
            // no operation
        }

        public TexoPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path can't be null or empty.", nameof(path));
            }

            if (!path.IsValidWildcardPath())
            {
                throw new FormatException("Invalid path format.");
            }

            this.path = path.NormalisePath();
            IsRelative = path.IsRelativePath();
            ContainsWildcard = BuildSegments();
        }

        public string Path => path;

        public bool ContainsWildcard { get; }

        public bool IsRelative { get; }

        public bool IsAbsolute => !IsRelative;

        public IImmutableList<PathSegment> Segments { get; private set; }

        private bool EndWithDirectorySeparator => path[path.Length - 1].IsDirectorySeparator();

        public TexoPath ToAbsolute()
        {
            return new TexoPath(GetAbsolutePath());
        }

        public Uri ToUri()
        {
            return new Uri(GetAbsolutePath(), UriKind.Absolute);
        }

        public override string ToString()
        {
            return path;
        }

        public TexoPath Combine(TexoPath second)
        {
            if (second.IsAbsolute)
            {
                return second;
            }

            return EndWithDirectorySeparator
                ? new TexoPath(path + second.path)
                : new TexoPath(path + System.IO.Path.DirectorySeparatorChar + second.path);
        }

        public TexoPath Combine(params TexoPath[] others)
        {
            TexoPath result = this;

            foreach (TexoPath otherPath in others)
            {
                result = result.Combine(otherPath);
            }

            return result;
        }

        public IImmutableList<string> GetItems()
        {
            return BuildItemList().ToImmutableList();
        }

        public IImmutableList<string> GetFiles()
        {
            return BuildItemList().Where(File.Exists).ToImmutableList();
        }

        public IImmutableList<string> GetDirectories()
        {
            return BuildItemList().Where(Directory.Exists).ToImmutableList();
        }

        public IImmutableList<string> GetTopDirectories()
        {
            var result = ImmutableList<string>.Empty.ToBuilder();
            Stack<string> directories = new Stack<string>();
            directories.Push(GetFixedPrefixPath());

            while (directories.Count > 0)
            {
                string directory = directories.Pop();

                foreach (string subDirectory in TexoDirectory.GetDirectories(directory))
                {
                    if (regularExpression.IsMatch(subDirectory))
                    {
                        result.Add(subDirectory);
                    }
                    else
                    {
                        directories.Push(subDirectory);
                    }
                }
            }

            return result.ToImmutable();
        }

        public IImmutableList<string> GetFilesFromDirectories()
        {
            return GetFilesFromDirectories(PathConstants.SEARCH_TERM_ALL, SearchOption.TopDirectoryOnly);
        }

        public IImmutableList<string> GetFilesFromDirectories(string searchPattern)
        {
            return GetFilesFromDirectories(searchPattern, SearchOption.TopDirectoryOnly);
        }

        public IImmutableList<string> GetFilesFromDirectories(string searchPattern, SearchOption options)
        {
            var result = ImmutableList<string>.Empty.ToBuilder();

            IEnumerable<string> directories =
                options == SearchOption.AllDirectories
                    ? GetTopDirectories()
                    : GetDirectories();


            foreach (string directory in directories)
            {
                result.AddRange(TexoDirectory.GetFiles(directory, searchPattern, options));
            }

            return result.ToImmutable();
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

            return string.Equals(path, other.path, StringComparison.OrdinalIgnoreCase);
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
                return path == null
                    ? 0
                    : StringComparer.OrdinalIgnoreCase.GetHashCode(path);
            }

            return GetAbsolutePath() == null
                ? 0
                : StringComparer.OrdinalIgnoreCase.GetHashCode(GetAbsolutePath());
        }

        public string GetFixedPrefixPath()
        {
            string result = IsRelative ? System.IO.Path.GetFullPath(".") : string.Empty;
            int segmentIndex = GetFirstWildcardOrLastIndex();

            for (int i = 0; i < segmentIndex; i++)
            {
                PathSegment segment = Segments[i];
                result = System.IO.Path.Combine(result, segment.Value);
            }

            return result;
        }

        public string GetAbsolutePath()
        {
            if (!ContainsWildcard)
            {
                return System.IO.Path.GetFullPath(path);
            }

            StringBuilder builder = new StringBuilder(GetFixedPrefixPath());
            int lastIndex = GetLastIndex();

            if (!builder[builder.Length - 1].IsDirectorySeparator())
            {
                builder.Append(System.IO.Path.DirectorySeparatorChar);
            }

            for (int i = GetFirstWildcardOrLastIndex(); i <= lastIndex; i++)
            {
                builder.Append(Segments[i].Value);
                builder.Append(System.IO.Path.DirectorySeparatorChar);
            }

            if (!EndWithDirectorySeparator)
            {
                builder.Remove(builder.Length - 1, 1);
            }

            return builder.ToString();
        }

        private List<string> BuildItemList()
        {
            return ProcessItems(new List<string>
            {
                GetFixedPrefixPath()
            });
        }

        private List<string> ProcessItems(List<string> directories)
        {
            int wildcardIndex = GetFirstWildcardOrLastIndex();
            int lastIndex = GetLastIndex();

            while (true)
            {
                PathSegment wildcard = Segments[wildcardIndex];

                if (wildcard.WildcardType == PathSegment.WildcardTypeEnum.Complex)
                {
                    return ProcessComplexItems(directories, wildcard, regularExpression);
                }

                if (wildcardIndex == lastIndex)
                {
                    return ProcessLeafItems(directories, wildcard);
                }

                List<string> newDirectories = new List<string>();

                foreach (string directory in directories)
                {
                    if (wildcard.WildcardType == PathSegment.WildcardTypeEnum.Simple)
                    {
                        newDirectories.AddRange(TexoDirectory.GetDirectories(directory, wildcard.Value,
                            SearchOption.TopDirectoryOnly));
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

        private bool BuildSegments()
        {
            StringBuilder regexBuilder = new StringBuilder();
            wildcardIndexes = new List<int>();

            var segments = ImmutableList<PathSegment>.Empty.ToBuilder();
            bool wildcard = false;
            int index = 0;

            foreach (string strSegment in path.SplitToPathSegments())
            {
                PathSegment segment = new PathSegment(
                    index == 0 && IsAbsolute
                        ? strSegment + System.IO.Path.DirectorySeparatorChar
                        : strSegment);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetLastIndex()
        {
            return Segments.Count - 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetFirstWildcardOrLastIndex()
        {
            return ContainsWildcard ? wildcardIndexes.First() : GetLastIndex();
        }

        private static List<string> ProcessComplexItems(IEnumerable<string> directories, PathSegment wildcard,
            Regex regularExpression)
        {
            List<string> newItems = new List<string>();
            int indexOfComplexWildcard =
                wildcard.Value.IndexOf(PathConstants.ANY_PATH_WILDCARD, StringComparison.OrdinalIgnoreCase);
            bool hasSimpleStart = false;
            string simpleStart = string.Empty;

            if (indexOfComplexWildcard > 0)
            {
                hasSimpleStart = true;
                simpleStart = wildcard.Value.Substring(0, indexOfComplexWildcard) +
                              PathConstants.WILDCARD_ANY_CHARACTER;
            }

            foreach (string directory in directories)
            {
                IEnumerable<string> subItems = hasSimpleStart
                    ? TexoDirectory.GetFileSystemEntries(directory, PathConstants.SEARCH_TERM_ALL, simpleStart)
                    : TexoDirectory.GetFileSystemEntries(directory, PathConstants.SEARCH_TERM_ALL,
                        SearchOption.AllDirectories);

                newItems.AddRange(subItems.Where(item => regularExpression.IsMatch(item)));
            }

            return newItems;
        }

        private static List<string> ProcessLeafItems(IEnumerable<string> directories, PathSegment leafSegment)
        {
            List<string> newItems = new List<string>();

            foreach (string directory in directories)
            {
                if (leafSegment.WildcardType == PathSegment.WildcardTypeEnum.None)
                {
                    newItems.Add(directory.CombinePathWith(leafSegment.Value));
                }
                else
                {
                    newItems.AddRange(Directory.GetFileSystemEntries(directory, leafSegment.Value,
                        SearchOption.TopDirectoryOnly));
                }
            }

            return newItems;
        }
    }
}