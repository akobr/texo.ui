namespace BeaverSoft.Texo.Commands.FileManager.Path
{
    public class Segment
    {
        public Segment(string segment)
        {
            Value = segment;
            ContainsWildcard = HasWildcard(segment);
        }

        public string Value { get; }

        public bool ContainsWildcard { get; }

        private static bool HasWildcard(string segment)
        {
            foreach (char character in segment)
            {
                if (character.IsWildcard())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
