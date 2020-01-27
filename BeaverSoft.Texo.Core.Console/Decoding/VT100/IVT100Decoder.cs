namespace BeaverSoft.Texo.Core.Console.Decoding.VT100
{
    public interface IVT100Decoder : IDecoder
    {
        void Subscribe(IVT100DecoderClient client);
    }
}
