using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.View
{
    public interface IClearViewOutputMessage : IServiceMessage
    {
        // no member
    }

    public class ClearViewOutputMessage : ServiceMessage, IClearViewOutputMessage
    {
        // no member
    }
}
