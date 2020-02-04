using System.Collections;

namespace BeaverSoft.Texo.Core.Console.Rendering
{
    class ViewBitArrayChanges : IViewChangesManager
    {
        private readonly BitArray changes;

        public ViewBitArrayChanges(int length)
        {
            changes = new BitArray(length);
        }

        public void AddChange(int index)
        {
            changes.Set(index, true);
        }

        public void AddChange(int start, int length)
        {
            for (int i = start; i < length; i++)
            {
                changes.Set(i, true);
            }
        }

        public void Reset()
        {
            changes.SetAll(false);
        }
    }
}
