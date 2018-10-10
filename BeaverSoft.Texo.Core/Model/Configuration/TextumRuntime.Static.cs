using System;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class TextumRuntime
    {
        public static TextumRuntime Empty { get; } = new TextumRuntime();

        public static TextumRuntime CreateDefault()
        {
            throw new NotImplementedException();
        }

        public static Builder CreateBuilder()
        {
            return Empty.ToBuilder();
        }
    }
}