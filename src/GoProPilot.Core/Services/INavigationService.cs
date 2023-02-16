using System;

namespace GoProPilot.Services;

public interface INavigationService
{
    void NavigateTo(Type type);
}
