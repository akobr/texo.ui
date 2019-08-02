using StrongBeaver.Core.Services;

namespace BeaverSoft.Texo.Core.Configuration
{
    public interface ISettingUpdatedMessage : IServiceMessage
    {
        TexoConfiguration Configuration { get; }
    }
}