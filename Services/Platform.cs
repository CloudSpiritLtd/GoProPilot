using System.Runtime.InteropServices;

namespace GoProPilot.Services;

public  static class Platform
{
    public static ICamera GetCameraAPI()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new GoProPilot.Services.Windows.Camera(null);

        return null;
    }
}
