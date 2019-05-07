using System.Threading.Tasks;

namespace BeaverSoft.Texo.Core.Transforming
{
    public interface IPipeline<TData> : ITransformation<TData>
    {
        void AddPipe(ITransformation<TData> pipe);

        bool AddPipeBefore(ITransformation<TData> pipeToAdd, ITransformation<TData> pipeReference);
    }
}
