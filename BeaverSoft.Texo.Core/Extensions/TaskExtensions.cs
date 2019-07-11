using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Extensions
{
    public static class TaskExtensions
    {
        public static bool IsFinished(this Task task)
        {
            return task != null && (int)task.Status > (int)TaskStatus.WaitingForChildrenToComplete;
        }
    }
}
