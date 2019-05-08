using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

namespace BeaverSoft.Text.Client.VisualStudio
{
    [Guid(WindowGuidString)]
    public class TexoToolWindow : ToolWindowPane
    {
        public const string WindowGuidString = "ca179975-59a3-4af6-b516-47017165b291";
        public const string Title = "Texo terminal";

        // "state" parameter is the object returned from MyPackage.InitializeToolWindowAsync
        public TexoToolWindow(TexoToolWindowState state)
            : base()
        {
            Caption = Title;
            BitmapImageMoniker = KnownMonikers.ImageIcon;
            Content = new EmptyControl();
        }
    }
}
