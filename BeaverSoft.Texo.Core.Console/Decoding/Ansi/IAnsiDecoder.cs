namespace BeaverSoft.Texo.Core.Console.Decoding.Ansi
{
    public interface IAnsiDecoder : IDecoder
    {
        void Subscribe(IAnsiDecoderClient client);
    }
}
