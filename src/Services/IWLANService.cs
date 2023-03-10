using System;
using DynamicData;
using GoProPilot.Models;

namespace GoProPilot.Services;

public interface IWLANService
{
    IObservable<IChangeSet<WLANDeviceModel, string>> Connect();
}
