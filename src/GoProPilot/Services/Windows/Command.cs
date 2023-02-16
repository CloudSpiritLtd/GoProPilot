using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoProPilot.Services.Windows;

public enum BLE_2_0_Command : Byte
{
    None = 0,
    SetShutter = 0x01,
    Sleep = 0x05,
    SetDateTime = 0x0D,
    GetDateTime = 0x0E,
    SetLivestreamMode = 0x15,
    APControl = 0x17,
    Media_HiLightMoment = 0x18,
    Get_Hardware_Info = 0x3C,
    Presets_LoadGroup = 0x3E,
    Presets_Load = 0x40,
    Analytics = 0x50,
    OpenGoPro = 0x51,
}
