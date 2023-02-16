using System.Runtime.InteropServices;

namespace GoProPilot.Services;

public  static class Platform
{
    public static ICameraService GetCameraAPI()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new GoProPilot.Services.Windows.CameraService(null);

        return null;
    }
}
