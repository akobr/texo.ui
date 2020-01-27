using System.Drawing;

namespace BeaverSoft.Texo.Core.Console.Decoding.VT100
{
    public interface IVT100DecoderClient : IDecoderClient
    {
        string GetDeviceCode();

        DeviceStatus GetDeviceStatus();

        void ResizeWindow(Size size);

        void MoveWindow(Point position);
    }
}
