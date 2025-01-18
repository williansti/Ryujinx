using SharpMetal.QuartzCore;
using System;
using System.Runtime.Versioning;

namespace Ryujinx.Ava.UI.Renderer
{
    [SupportedOSPlatform("macos")]
    public class EmbeddedWindowMetal : EmbeddedWindow
    {
        public CAMetalLayer CreateSurface()
        {
            if (OperatingSystem.IsMacOS())
            {
                return new CAMetalLayer(MetalLayer);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
