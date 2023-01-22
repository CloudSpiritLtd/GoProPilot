using System;
using DynamicData;
using GoProPilot.Models;

namespace GoProPilot.Services;

public interface IWLANService
{
    IObservable<IChangeSet<WLANDeviceWrapper, string>> Connect();
}
